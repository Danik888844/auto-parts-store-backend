using System.Net;
using AutoMapper;
using AutoParts.Core.GeneralHelpers;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Contexts;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels.StockMovement;
using AutoParts.DataAccess.Models.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoParts.Business.Cqrs.StockMovements;

public class StockMovementCreateCommand : IRequest<IDataResult<object>>
{
    public StockMovementFormDto Form { get; }
    public string? UserId { get; }

    public StockMovementCreateCommand(StockMovementFormDto form, string? userId = null)
    {
        Form = form;
        UserId = userId;
    }

    public class StockMovementCreateCommandHandler : IRequestHandler<StockMovementCreateCommand, IDataResult<object>>
    {
        private readonly AutoPartsStoreDb _db;
        private readonly IValidator<StockMovementFormDto> _validator;
        private readonly IXssRepository _xssRepository;
        private readonly IMapper _mapper;

        public StockMovementCreateCommandHandler(
            AutoPartsStoreDb db,
            IValidator<StockMovementFormDto> validator,
            IXssRepository xssRepository,
            IMapper mapper)
        {
            _db = db;
            _validator = validator;
            _xssRepository = xssRepository;
            _mapper = mapper;
        }

        public async Task<IDataResult<object>> Handle(StockMovementCreateCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validation.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            var product = await _db.Set<Product>().FirstOrDefaultAsync(p => p.Id == request.Form.ProductId && !p.IsDeleted, cancellationToken);
            if (product == null)
                return new ErrorDataResult<object>("Product not found", HttpStatusCode.NotFound);

            if (request.Form.Quantity <= 0)
                return new ErrorDataResult<object>("Quantity must be greater than 0", HttpStatusCode.BadRequest);

            await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var stock = await _db.Set<Stock>().FirstOrDefaultAsync(s => s.ProductId == request.Form.ProductId && !s.IsDeleted, cancellationToken);
                if (stock == null)
                {
                    stock = new Stock
                    {
                        ProductId = request.Form.ProductId,
                        Quantity = 0,
                        CreatedDate = DateTime.UtcNow.Ticks,
                        ModifiedDate = DateTime.UtcNow.Ticks
                    };
                    _db.Set<Stock>().Add(stock);
                    await _db.SaveChangesAsync(cancellationToken);
                }

                switch (request.Form.Type)
                {
                    case StockMovementType.Out:
                        if (stock.Quantity < request.Form.Quantity)
                            return new ErrorDataResult<object>($"Insufficient stock. Available: {stock.Quantity}", HttpStatusCode.BadRequest);
                        stock.Quantity -= request.Form.Quantity;
                        break;
                    case StockMovementType.In:
                        stock.Quantity += request.Form.Quantity;
                        break;
                    case StockMovementType.Adjust:
                        stock.Quantity = request.Form.Quantity;
                        break;
                }

                stock.ModifiedDate = DateTime.UtcNow.Ticks;
                _db.Set<Stock>().Update(stock);

                var movement = new StockMovement
                {
                    ProductId = request.Form.ProductId,
                    Type = request.Form.Type,
                    Quantity = request.Form.Quantity,
                    OccurredAt = DateTime.UtcNow,
                    Reason = string.IsNullOrWhiteSpace(request.Form.Reason) ? null : _xssRepository.Execute(request.Form.Reason),
                    DocumentNo = string.IsNullOrWhiteSpace(request.Form.DocumentNo) ? null : _xssRepository.Execute(request.Form.DocumentNo),
                    SupplierId = request.Form.SupplierId,
                    UserId = request.UserId,
                    CreatedDate = DateTime.UtcNow.Ticks,
                    ModifiedDate = DateTime.UtcNow.Ticks
                };
                _db.Set<StockMovement>().Add(movement);
                await _db.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                var withIncludes = await _db.Set<StockMovement>()
                    .Include(sm => sm.Product)
                    .Include(sm => sm.Supplier)
                    .FirstAsync(sm => sm.Id == movement.Id, cancellationToken);
                return new SuccessDataResult<object>(_mapper.Map<StockMovementDto>(withIncludes), "Movement created");
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
