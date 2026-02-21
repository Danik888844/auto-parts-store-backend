using System.Net;
using AutoMapper;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Contexts;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels.Sale;
using AutoParts.DataAccess.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoParts.Business.Cqrs.Sales;

/// <summary>
/// Завершает продажу из статуса Draft: списывает остатки, создаёт движения, переводит в Completed.
/// </summary>
public class CompleteSaleCommand : IRequest<IDataResult<object>>
{
    public int SaleId { get; }
    public string? UserId { get; }

    public CompleteSaleCommand(int saleId, string? userId = null)
    {
        SaleId = saleId;
        UserId = userId;
    }

    public class CompleteSaleCommandHandler : IRequestHandler<CompleteSaleCommand, IDataResult<object>>
    {
        private readonly AutoPartsStoreDb _db;
        private readonly IMapper _mapper;

        public CompleteSaleCommandHandler(AutoPartsStoreDb db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IDataResult<object>> Handle(CompleteSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _db.Set<Sale>()
                .Include(s => s.Client)
                .Include(s => s.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(s => s.Id == request.SaleId && !s.IsDeleted, cancellationToken);

            if (sale == null)
                return new ErrorDataResult<object>("Sale not found", HttpStatusCode.NotFound);

            if (sale.Status != SaleStatus.Draft)
                return new ErrorDataResult<object>("Only draft sales can be completed", HttpStatusCode.BadRequest);

            var items = sale.Items.Where(i => !i.IsDeleted && i.Quantity > 0).ToList();
            if (items.Count == 0)
                return new ErrorDataResult<object>("Sale has no items to complete", HttpStatusCode.BadRequest);

            var productIds = items.Select(i => i.ProductId).Distinct().ToList();
            var stocks = await _db.Set<Stock>()
                .Where(s => productIds.Contains(s.ProductId) && !s.IsDeleted)
                .ToDictionaryAsync(s => s.ProductId, cancellationToken);

            foreach (var item in items)
            {
                if (!stocks.TryGetValue(item.ProductId, out var stock))
                {
                    stock = new Stock
                    {
                        ProductId = item.ProductId,
                        Quantity = 0,
                        CreatedDate = DateTime.UtcNow.Ticks,
                        ModifiedDate = DateTime.UtcNow.Ticks
                    };
                    _db.Set<Stock>().Add(stock);
                    await _db.SaveChangesAsync(cancellationToken);
                    stocks[item.ProductId] = stock;
                }

                if (stock.Quantity < item.Quantity)
                {
                    var product = item.Product;
                    return new ErrorDataResult<object>(
                        $"Insufficient stock for product {product?.Name} (SKU: {product?.Sku}). Available: {stock.Quantity}, requested: {item.Quantity}",
                        HttpStatusCode.BadRequest);
                }
            }

            await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                foreach (var item in items)
                {
                    var stock = stocks[item.ProductId];
                    stock.Quantity -= item.Quantity;
                    stock.ModifiedDate = DateTime.UtcNow.Ticks;
                    _db.Set<Stock>().Update(stock);

                    var movement = new StockMovement
                    {
                        ProductId = item.ProductId,
                        Type = StockMovementType.Out,
                        Quantity = item.Quantity,
                        OccurredAt = DateTime.UtcNow,
                        Reason = "Sale",
                        DocumentNo = $"SALE-{sale.Id}",
                        UserId = request.UserId,
                        CreatedDate = DateTime.UtcNow.Ticks,
                        ModifiedDate = DateTime.UtcNow.Ticks
                    };
                    _db.Set<StockMovement>().Add(movement);
                }

                sale.Status = SaleStatus.Completed;
                sale.ModifiedDate = DateTime.UtcNow.Ticks;
                _db.Set<Sale>().Update(sale);
                await _db.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                var updated = await _db.Set<Sale>()
                    .Include(s => s.Client)
                    .Include(s => s.Items)
                    .ThenInclude(i => i.Product)
                    .FirstAsync(s => s.Id == sale.Id, cancellationToken);
                return new SuccessDataResult<object>(_mapper.Map<SaleDto>(updated), "Sale completed");
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
