using System.Net;
using AutoMapper;
using AutoParts.Core.GeneralHelpers;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DtoModels.ProductCompatibility;
using FluentValidation;
using MediatR;

namespace AutoParts.Business.Cqrs.ProductCompatibilities;

public class ProductCompatibilityEditCommand : IRequest<IDataResult<object>>
{
    public int Id { get; }
    public ProductCompatibilityFormDto Form { get; }

    public ProductCompatibilityEditCommand(int id, ProductCompatibilityFormDto form)
    {
        Id = id;
        Form = form;
    }

    public class ProductCompatibilityEditCommandHandler : IRequestHandler<ProductCompatibilityEditCommand, IDataResult<object>>
    {
        private readonly IProductCompatibilityDal _compatibilityDal;
        private readonly IProductDal _productDal;
        private readonly IVehicleDal _vehicleDal;
        private readonly IValidator<ProductCompatibilityFormDto> _validator;
        private readonly IXssRepository _xssRepository;
        private readonly IMapper _mapper;

        public ProductCompatibilityEditCommandHandler(
            IProductCompatibilityDal compatibilityDal,
            IProductDal productDal,
            IVehicleDal vehicleDal,
            IValidator<ProductCompatibilityFormDto> validator,
            IXssRepository xssRepository,
            IMapper mapper)
        {
            _compatibilityDal = compatibilityDal;
            _productDal = productDal;
            _vehicleDal = vehicleDal;
            _validator = validator;
            _xssRepository = xssRepository;
            _mapper = mapper;
        }

        public async Task<IDataResult<object>> Handle(ProductCompatibilityEditCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validation.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            var source = await _compatibilityDal.GetAsync(pc => pc.Id == request.Id && !pc.IsDeleted);
            if (source == null)
                return new ErrorDataResult<object>("Record not found", HttpStatusCode.NotFound);

            var productExists = await _productDal.AnyAsync(p => p.Id == request.Form.ProductId && !p.IsDeleted);
            if (!productExists)
                return new ErrorDataResult<object>("Product not found", HttpStatusCode.NotFound);

            var vehicleExists = await _vehicleDal.AnyAsync(v => v.Id == request.Form.VehicleId && !v.IsDeleted);
            if (!vehicleExists)
                return new ErrorDataResult<object>("Vehicle not found", HttpStatusCode.NotFound);

            var duplicate = await _compatibilityDal.AnyAsync(pc =>
                pc.ProductId == request.Form.ProductId &&
                pc.VehicleId == request.Form.VehicleId &&
                pc.Id != request.Id &&
                !pc.IsDeleted);
            if (duplicate)
                return new ErrorDataResult<object>("This product-vehicle compatibility already exists", HttpStatusCode.Conflict);

            source.ProductId = request.Form.ProductId;
            source.VehicleId = request.Form.VehicleId;
            source.Comment = string.IsNullOrWhiteSpace(request.Form.Comment)
                ? null
                : _xssRepository.Execute(request.Form.Comment);

            await _compatibilityDal.UpdateAsync(source);

            var dto = await _compatibilityDal.GetWithIncludesAsync(source.Id);
            return new SuccessDataResult<object>(_mapper.Map<ProductCompatibilityDto>(dto), "Compatibility edited");
        }
    }
}
