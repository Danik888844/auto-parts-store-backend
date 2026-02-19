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

public class RefundSaleCommand : IRequest<IDataResult<object>>
{
    public int SaleId { get; }
    public string? UserId { get; }

    public RefundSaleCommand(int saleId, string? userId = null)
    {
        SaleId = saleId;
        UserId = userId;
    }

    public class RefundSaleCommandHandler : IRequestHandler<RefundSaleCommand, IDataResult<object>>
    {
        private readonly AutoPartsStoreDb _db;
        private readonly IMapper _mapper;

        public RefundSaleCommandHandler(AutoPartsStoreDb db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IDataResult<object>> Handle(RefundSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _db.Set<Sale>()
                .Include(s => s.Client)
                .Include(s => s.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(s => s.Id == request.SaleId && !s.IsDeleted, cancellationToken);

            if (sale == null)
                return new ErrorDataResult<object>("Sale not found", HttpStatusCode.NotFound);

            if (sale.Status == SaleStatus.Refunded)
                return new ErrorDataResult<object>("Sale is already refunded", HttpStatusCode.BadRequest);

            if (sale.Status != SaleStatus.Completed)
                return new ErrorDataResult<object>("Only completed sales can be refunded", HttpStatusCode.BadRequest);

            await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                foreach (var item in sale.Items.Where(i => !i.IsDeleted && i.Quantity > 0))
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

                    stock.Quantity += item.Quantity;
                    stock.ModifiedDate = DateTime.UtcNow.Ticks;
                    _db.Set<Stock>().Update(stock);

                    var movement = new StockMovement
                    {
                        ProductId = item.ProductId,
                        Type = StockMovementType.In,
                        Quantity = item.Quantity,
                        OccurredAt = DateTime.UtcNow,
                        Reason = "Refund",
                        DocumentNo = $"REFUND-SALE-{sale.Id}",
                        UserId = request.UserId,
                        CreatedDate = DateTime.UtcNow.Ticks,
                        ModifiedDate = DateTime.UtcNow.Ticks
                    };
                    _db.Set<StockMovement>().Add(movement);
                }

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
                return new SuccessDataResult<object>(_mapper.Map<SaleDto>(updated), "Sale refunded");
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
