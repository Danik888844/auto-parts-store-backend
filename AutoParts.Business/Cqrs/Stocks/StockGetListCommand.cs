using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using AutoParts.Core.GeneralHelpers;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels;
using AutoParts.DataAccess.Models.DtoModels.Stock;
using FluentValidation;
using MediatR;

namespace AutoParts.Business.Cqrs.Stocks;

public class StockGetListCommand : IRequest<IDataResult<object>>
{
    public StockListFormDto Form { get; }

    public StockGetListCommand(StockListFormDto form)
    {
        Form = form;
    }

    public class StockGetListCommandHandler : IRequestHandler<StockGetListCommand, IDataResult<object>>
    {
        private readonly IStockDal _dal;
        private readonly IValidator<StockListFormDto> _validator;
        private readonly IMapper _mapper;

        public StockGetListCommandHandler(IStockDal dal, IValidator<StockListFormDto> validator, IMapper mapper)
        {
            _dal = dal;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<IDataResult<object>> Handle(StockGetListCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validation.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            Expression<Func<Stock, bool>>? filter = null;
            if (!string.IsNullOrWhiteSpace(request.Form.Search))
            {
                var s = request.Form.Search.Trim().ToLower();
                filter = st => (st.Product != null && (st.Product.Name.ToLower().Contains(s) || st.Product.Sku.ToLower().Contains(s)));
            }

            var source = await _dal.GetPagedWithProductAsync(filter, request.Form.PageNumber, request.Form.ViewSize);
            var items = source.Items.Select(i => _mapper.Map<StockDto>(i)).ToList();

            return new SuccessDataResult<object>(new PaginationReturnListDto<StockDto>
            {
                Items = items,
                Pagination = new PaginationReturnModel()
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
