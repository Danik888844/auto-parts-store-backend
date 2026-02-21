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

public class RefundSaleItemCommand : IRequest<IDataResult<object>>
{
    public int SaleId { get; }
    public int SaleItemId { get; }
    public int Quantity { get; }
    public string? UserId { get; }

    /// <summary>
    /// Refund part or all of a sale item. If quantity is 0 or not specified, full line is refunded.
    /// </summary>
    public RefundSaleItemCommand(int saleId, int saleItemId, int quantity = 0, string? userId = null)
    {
        SaleId = saleId;
        SaleItemId = saleItemId;
        Quantity = quantity;
        UserId = userId;
    }

    public class RefundSaleItemCommandHandler : IRequestHandler<RefundSaleItemCommand, IDataResult<object>>
    {
        private readonly AutoPartsStoreDb _db;
        private readonly IMapper _mapper;

        public RefundSaleItemCommandHandler(AutoPartsStoreDb db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IDataResult<object>> Handle(RefundSaleItemCommand request, CancellationToken cancellationToken)
        {
            var sale = await _db.Set<Sale>()
                .Include(s => s.Client)
                .Include(s => s.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(s => s.Id == request.SaleId && !s.IsDeleted, cancellationToken);

            if (sale == null)
                return new ErrorDataResult<object>("Sale not found", HttpStatusCode.NotFound);

            if (sale.Status == SaleStatus.Refunded)
                return new ErrorDataResult<object>("Sale is already fully refunded", HttpStatusCode.BadRequest);

            if (sale.Status != SaleStatus.Completed)
                return new ErrorDataResult<object>("Only completed sales can be refunded", HttpStatusCode.BadRequest);

            var item = sale.Items.FirstOrDefault(i => i.Id == request.SaleItemId && !i.IsDeleted);
            if (item == null)
                return new ErrorDataResult<object>("Sale item not found", HttpStatusCode.NotFound);

            var refundQty = request.Quantity <= 0 ? item.Quantity : Math.Min(request.Quantity, item.Quantity);
            if (refundQty <= 0)
                return new ErrorDataResult<object>("Nothing to refund", HttpStatusCode.BadRequest);

            await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var stock = await _db.Set<Stock>().FirstOrDefaultAsync(s => s.ProductId == item.ProductId && !s.IsDeleted, cancellationToken);
                if (stock == null)
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
                }

                stock.Quantity += refundQty;
                stock.ModifiedDate = DateTime.UtcNow.Ticks;
                _db.Set<Stock>().Update(stock);

                var movement = new StockMovement
                {
                    ProductId = item.ProductId,
                    Type = StockMovementType.In,
                    Quantity = refundQty,
                    OccurredAt = DateTime.UtcNow,
                    Reason = "Refund (partial)",
                    DocumentNo = $"REFUND-SALE-{sale.Id}-ITEM-{item.Id}",
                    UserId = request.UserId,
                    CreatedDate = DateTime.UtcNow.Ticks,
                    ModifiedDate = DateTime.UtcNow.Ticks
                };
                _db.Set<StockMovement>().Add(movement);

                item.Quantity -= refundQty;
                item.LineTotal = item.Price * item.Quantity;
                item.ModifiedDate = DateTime.UtcNow.Ticks;
                _db.Set<SaleItem>().Update(item);

                sale.Total = sale.Items.Where(i => !i.IsDeleted).Sum(i => i.LineTotal);
                if (sale.Total == 0)
                    sale.Status = SaleStatus.Refunded;
                sale.ModifiedDate = DateTime.UtcNow.Ticks;
                _db.Set<Sale>().Update(sale);

                await _db.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                var updated = await _db.Set<Sale>()
                    .Include(s => s.Client)
                    .Include(s => s.Items)
                    .ThenInclude(i => i.Product)
                    .FirstAsync(s => s.Id == sale.Id, cancellationToken);
                return new SuccessDataResult<object>(_mapper.Map<SaleDto>(updated), "Item refunded");
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
