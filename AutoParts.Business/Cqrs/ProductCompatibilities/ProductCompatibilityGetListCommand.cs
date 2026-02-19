using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels;
using AutoParts.DataAccess.Models.DtoModels.ProductCompatibility;
using FluentValidation;
using MediatR;

namespace AutoParts.Business.Cqrs.ProductCompatibilities;

public class ProductCompatibilityGetListCommand : IRequest<IDataResult<object>>
{
    public PaginationFormDto Form { get; }

    public ProductCompatibilityGetListCommand(PaginationFormDto form)
    {
        Form = form;
    }

    public class ProductCompatibilityGetListCommandHandler : IRequestHandler<ProductCompatibilityGetListCommand, IDataResult<object>>
    {
        private readonly IProductCompatibilityDal _compatibilityDal;
        private readonly IValidator<PaginationFormDto> _validator;
        private readonly IMapper _mapper;

        public ProductCompatibilityGetListCommandHandler(
            IProductCompatibilityDal compatibilityDal,
            IValidator<PaginationFormDto> validator,
            IMapper mapper)
        {
            _compatibilityDal = compatibilityDal;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<IDataResult<object>> Handle(ProductCompatibilityGetListCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validation.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            Expression<Func<ProductCompatibility, bool>>? filter = null;
            if (!string.IsNullOrWhiteSpace(request.Form.Search))
            {
                var s = request.Form.Search.Trim().ToLower();
                filter = pc =>
                    (pc.Product != null && pc.Product.Name.ToLower().Contains(s)) ||
                    (pc.Product != null && pc.Product.Sku.ToLower().Contains(s)) ||
                    (pc.Vehicle != null && pc.Vehicle.Model != null && pc.Vehicle.Model.Name.ToLower().Contains(s)) ||
                    (pc.Vehicle != null && pc.Vehicle.Model != null && pc.Vehicle.Model.Brand != null && pc.Vehicle.Model.Brand.Name.ToLower().Contains(s)) ||
                    (pc.Comment != null && pc.Comment.ToLower().Contains(s));
            }

            var source = await _compatibilityDal.GetPagedWithIncludesAsync(filter, request.Form.PageNumber, request.Form.ViewSize);
            var items = source.Items.Select(i => _mapper.Map<ProductCompatibilityDto>(i)).ToList();

            return new SuccessDataResult<object>(new PaginationReturnListDto<ProductCompatibilityDto>
            {
                Items = items,
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
