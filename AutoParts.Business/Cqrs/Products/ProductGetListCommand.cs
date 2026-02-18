using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels;
using AutoParts.DataAccess.Models.DtoModels.Product;
using FluentValidation;
using MediatR;

namespace AutoParts.Business.Cqrs.Products;

public class ProductGetListCommand : IRequest<IDataResult<object>>
{
    public PaginationFormDto Form { get; }

    public ProductGetListCommand(PaginationFormDto form)
    {
        Form = form;
    }

    public class ProductGetListCommandHandler : IRequestHandler<ProductGetListCommand, IDataResult<object>>
    {
        private readonly IProductDal _productDal;
        private readonly IValidator<PaginationFormDto> _validator;
        private readonly IMapper _mapper;

        public ProductGetListCommandHandler(IProductDal productDal,
            IValidator<PaginationFormDto> validator, IMapper mapper)
        {
            _productDal = productDal;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<IDataResult<object>> Handle(ProductGetListCommand request, CancellationToken cancellationToken)
        {
            var validationOfForm = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validationOfForm.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validationOfForm.Errors.Select(e => e.ErrorMessage).ToList());

            Expression<Func<Product, bool>>? filter = null;
            if (!string.IsNullOrWhiteSpace(request.Form.Search))
            {
                var s = request.Form.Search.Trim().ToLower();
                filter = p => p.Name.ToLower().Contains(s)
                    || p.Sku.ToLower().Contains(s)
                    || (p.Description != null && p.Description.ToLower().Contains(s));
            }

            var source = await _productDal.GetPagedWithIncludesAsync(filter, request.Form.PageNumber, request.Form.ViewSize);

            var products = source.Items.Select(i => _mapper.Map<ProductDto>(i)).ToList();

            return new SuccessDataResult<object>(new PaginationReturnListDto<ProductDto>
            {
                Items = products,
                Pagination = new PaginationReturnModel
                {
                    CurrentPage = request.Form.PageNumber,
                    PageSize = request.Form.ViewSize,
                    TotalItems = source.TotalCount,
                    TotalPages = source.TotalPages
                },
            });
        }
    }
}
