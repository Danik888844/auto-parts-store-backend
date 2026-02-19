using System.Net;
using AutoMapper;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Contexts;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels.Sale;
using AutoParts.DataAccess.Models.DtoModels.SaleItem;
using AutoParts.DataAccess.Models.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoParts.Business.Cqrs.Sales;

public class SaleCreateCommand : IRequest<IDataResult<object>>
{
    public SaleFormDto Form { get; }
    public string? UserId { get; }

    public SaleCreateCommand(SaleFormDto form, string? userId = null)
    {
        Form = form;
        UserId = userId;
    }

    public class SaleCreateCommandHandler : IRequestHandler<SaleCreateCommand, IDataResult<object>>
    {
        private readonly AutoPartsStoreDb _db;
        private readonly IValidator<SaleFormDto> _validator;
        private readonly IMapper _mapper;

        public SaleCreateCommandHandler(AutoPartsStoreDb db, IValidator<SaleFormDto> validator, IMapper mapper)
        {
            _db = db;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<IDataResult<object>> Handle(SaleCreateCommand request, CancellationToken cancellationToken)
        {
            if (request.Form.Items == null || request.Form.Items.Count == 0)
                return new ErrorDataResult<object>("Sale must have at least one item", HttpStatusCode.BadRequest);

            var validation = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validation.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            if (request.Form.ClientId.HasValue)
            {
                var clientExists = await _db.Set<Client>().AnyAsync(c => c.Id == request.Form.ClientId.Value && !c.IsDeleted, cancellationToken);
                if (!clientExists)
                    return new ErrorDataResult<object>("Client not found", HttpStatusCode.NotFound);
            }

            var productIds = request.Form.Items.Select(i => i.ProductId).Distinct().ToList();
            var products = await _db.Set<Product>()
                .Where(p => productIds.Contains(p.Id) && !p.IsDeleted && p.IsActive)
                .ToDictionaryAsync(p => p.Id, cancellationToken);

            if (products.Count != productIds.Count)
                return new ErrorDataResult<object>("One or more products not found or inactive", HttpStatusCode.BadRequest);

            var stocks = await _db.Set<Stock>()
                .Where(s => productIds.Contains(s.ProductId) && !s.IsDeleted)
                .ToDictionaryAsync(s => s.ProductId, cancellationToken);

            decimal total = 0;
            var saleItemsToAdd = new List<SaleItem>();
            var movementsToAdd = new List<StockMovement>();
            var stockUpdates = new List<Stock>();

            foreach (var item in request.Form.Items)
            {
                if (item.Quantity <= 0)
                    return new ErrorDataResult<object>($"Invalid quantity for product {item.ProductId}", HttpStatusCode.BadRequest);

                var product = products[item.ProductId];
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
                    stocks[item.ProductId] = stock;
                }

                if (stock.Quantity < item.Quantity)
                    return new ErrorDataResult<object>($"Insufficient stock for product {product.Name} (SKU: {product.Sku}). Available: {stock.Quantity}, requested: {item.Quantity}", HttpStatusCode.BadRequest);

                var price = product.Price;
                var lineTotal = price * item.Quantity;
                total += lineTotal;

                saleItemsToAdd.Add(new SaleItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = price,
                    LineTotal = lineTotal,
                    CreatedDate = DateTime.UtcNow.Ticks,
                    ModifiedDate = DateTime.UtcNow.Ticks
                });

                stock.Quantity -= item.Quantity;
                stock.ModifiedDate = DateTime.UtcNow.Ticks;
                stockUpdates.Add(stock);

                movementsToAdd.Add(new StockMovement
                {
                    ProductId = item.ProductId,
                    Type = StockMovementType.Out,
                    Quantity = item.Quantity,
                    OccurredAt = DateTime.UtcNow,
                    Reason = "Sale",
                    DocumentNo = null,
                    UserId = request.UserId,
                    CreatedDate = DateTime.UtcNow.Ticks,
                    ModifiedDate = DateTime.UtcNow.Ticks
                });
            }

            await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var sale = new Sale
                {
                    SoldAt = DateTime.UtcNow,
                    ClientId = request.Form.ClientId,
                    UserId = request.UserId ?? "",
                    PaymentType = request.Form.PaymentType,
                    Status = SaleStatus.Completed,
                    Total = total,
                    CreatedDate = DateTime.UtcNow.Ticks,
                    ModifiedDate = DateTime.UtcNow.Ticks
                };
                _db.Set<Sale>().Add(sale);
                await _db.SaveChangesAsync(cancellationToken);

                foreach (var si in saleItemsToAdd)
                {
                    si.SaleId = sale.Id;
                    _db.Set<SaleItem>().Add(si);
                }
                await _db.SaveChangesAsync(cancellationToken);

                foreach (var mov in movementsToAdd)
                {
                    mov.DocumentNo = $"SALE-{sale.Id}";
                    _db.Set<StockMovement>().Add(mov);
                }
                foreach (var st in stockUpdates)
                    _db.Set<Stock>().Update(st);

                await _db.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                var withIncludes = await _db.Set<Sale>()
                    .Include(s => s.Client)
                    .Include(s => s.Items)
                    .ThenInclude(i => i.Product)
                    .FirstAsync(s => s.Id == sale.Id, cancellationToken);
                return new SuccessDataResult<object>(_mapper.Map<SaleDto>(withIncludes), "Sale created");
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
