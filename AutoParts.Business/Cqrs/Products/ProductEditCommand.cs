using System.Net;
using AutoMapper;
using AutoParts.Core.GeneralHelpers;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DtoModels.Product;
using FluentValidation;
using MediatR;

namespace AutoParts.Business.Cqrs.Products;

public class ProductEditCommand : IRequest<IDataResult<object>>
{
    public int Id { get; }
    public ProductFormUpdateDto Form { get; }

    public ProductEditCommand(int id, ProductFormUpdateDto form)
    {
        Id = id;
        Form = form;
    }

    public class ProductEditCommandHandler : IRequestHandler<ProductEditCommand, IDataResult<object>>
    {
        private readonly IValidator<ProductFormUpdateDto> _validator;
        private readonly IProductDal _productDal;
        private readonly IXssRepository _xssRepository;
        private readonly IMapper _mapper;

        public ProductEditCommandHandler(IValidator<ProductFormUpdateDto> validator, IProductDal productDal,
            IXssRepository xssRepository, IMapper mapper)
        {
            _validator = validator;
            _productDal = productDal;
            _xssRepository = xssRepository;
            _mapper = mapper;
        }

        public async Task<IDataResult<object>> Handle(ProductEditCommand request, CancellationToken cancellationToken)
        {
            var validationOfForm = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validationOfForm.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validationOfForm.Errors.Select(e => e.ErrorMessage).ToList());

            var source = await _productDal.GetAsync(p => p.Id == request.Id && !p.IsDeleted);
            if (source == null)
                return new ErrorDataResult<object>("Record not found", HttpStatusCode.NotFound);

            source.Name = _xssRepository.Execute(request.Form.Name);
            source.CategoryId = request.Form.CategoryId;
            source.ManufacturerId = request.Form.ManufacturerId;
            source.Price = request.Form.Price;
            source.PurchasePrice = request.Form.PurchasePrice;
            source.Description = string.IsNullOrWhiteSpace(request.Form.Description)
                ? null
                : _xssRepository.Execute(request.Form.Description);
            source.IsActive = request.Form.IsActive;

            await _productDal.UpdateAsync(source);

            var productDto = _mapper.Map<ProductDto>(source);

            return new SuccessDataResult<object>(productDto, $"\"{source.Name}\" edited");
        }
    }
}
