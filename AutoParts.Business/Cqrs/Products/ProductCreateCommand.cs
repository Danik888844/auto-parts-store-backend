using System.Net;
using AutoMapper;
using AutoParts.Core.GeneralHelpers;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels.Product;
using FluentValidation;
using MediatR;

namespace AutoParts.Business.Cqrs.Products;

public class ProductCreateCommand : IRequest<IDataResult<object>>
{
    public ProductFormCreateDto Form { get; }

    public ProductCreateCommand(ProductFormCreateDto form)
    {
        Form = form;
    }

    public class ProductCreateCommandHandler : IRequestHandler<ProductCreateCommand, IDataResult<object>>
    {
        private readonly IProductDal _productDal;
        private readonly IValidator<ProductFormCreateDto> _validator;
        private readonly IXssRepository _xssRepository;
        private readonly IMapper _mapper;

        public ProductCreateCommandHandler(IProductDal productDal, IValidator<ProductFormCreateDto> validator,
            IXssRepository xssRepository, IMapper mapper)
        {
            _productDal = productDal;
            _validator = validator;
            _xssRepository = xssRepository;
            _mapper = mapper;
        }

        public async Task<IDataResult<object>> Handle(ProductCreateCommand request, CancellationToken cancellationToken)
        {
            var validationOfForm = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validationOfForm.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validationOfForm.Errors.Select(e => e.ErrorMessage).ToList());

            var existsBySku = await _productDal.AnyAsync(p => p.Sku == request.Form.Sku.Trim() && !p.IsDeleted);
            if (existsBySku)
                return new ErrorDataResult<object>("Product with this SKU already exists", HttpStatusCode.Conflict);

            request.Form.Sku = _xssRepository.Execute(request.Form.Sku.Trim());
            request.Form.Name = _xssRepository.Execute(request.Form.Name);
            if (!string.IsNullOrWhiteSpace(request.Form.Description))
                request.Form.Description = _xssRepository.Execute(request.Form.Description);

            var source = _mapper.Map<Product>(request.Form);

            await _productDal.AddAsync(source);

            var productDto = _mapper.Map<ProductDto>(source);

            return new SuccessDataResult<object>(productDto, $"Created: {source.Name}");
        }
    }
}
