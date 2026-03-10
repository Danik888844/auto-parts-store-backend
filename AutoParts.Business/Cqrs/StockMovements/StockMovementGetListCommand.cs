using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using AutoParts.Business.Services.Identity;
using AutoParts.Core.GeneralHelpers;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels;
using AutoParts.DataAccess.Models.DtoModels.IdentityModels;
using AutoParts.DataAccess.Models.DtoModels.StockMovement;
using AutoParts.DataAccess.Models.Enums;
using FluentValidation;
using MediatR;

namespace AutoParts.Business.Cqrs.StockMovements;

public class StockMovementGetListCommand : IRequest<IDataResult<object>>
{
    public StockMovementListFormDto Form { get; }

    public StockMovementGetListCommand(StockMovementListFormDto form)
    {
        Form = form;
    }

    public class StockMovementGetListCommandHandler : IRequestHandler<StockMovementGetListCommand, IDataResult<object>>
    {
        private readonly IStockMovementDal _dal;
        private readonly IValidator<StockMovementListFormDto> _validator;
        private readonly IMapper _mapper;
        private readonly IIdentityUserApiClient _identityUserApi;

        public StockMovementGetListCommandHandler(
            IStockMovementDal dal,
            IValidator<StockMovementListFormDto> validator,
            IMapper mapper,
            IIdentityUserApiClient identityUserApi)
        {
            _dal = dal;
            _validator = validator;
            _mapper = mapper;
            _identityUserApi = identityUserApi;
        }

        public async Task<IDataResult<object>> Handle(StockMovementGetListCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validation.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            var productId = request.Form.ProductId;
            var type = request.Form.Type;
            var dateFrom = request.Form.DateFrom?.Date;
            var dateTo = request.Form.DateTo?.Date.AddDays(1);

            Expression<Func<StockMovement, bool>> filter = sm =>
                (!productId.HasValue || sm.ProductId == productId.Value)
                && (!type.HasValue || sm.Type == type.Value)
                && (!dateFrom.HasValue || sm.OccurredAt >= dateFrom.Value)
                && (!dateTo.HasValue || sm.OccurredAt < dateTo.Value);

            var source = await _dal.GetPagedWithIncludesAsync(filter, request.Form.PageNumber, request.Form.ViewSize);
            var items = source.Items.Select(i => _mapper.Map<StockMovementDto>(i)).ToList();

            // Заполняем информацию о пользователях через Identity API
            var userIds = items
                .Select(i => i.UserId)
                .Where(id => !string.IsNullOrEmpty(id))
                .Distinct()
                .ToList();

            if (userIds.Count > 0)
            {
                var displayNames = await _identityUserApi.GetDisplayNamesAsync(userIds, cancellationToken);

                foreach (var dto in items)
                {
                    if (!string.IsNullOrEmpty(dto.UserId) &&
                        displayNames.TryGetValue(dto.UserId, out var name))
                    {
                        dto.UserDto = new UserDto
                        {
                            Id = dto.UserId!,
                            UserName = name
                        };
                    }
                }
            }

            return new SuccessDataResult<object>(new PaginationReturnListDto<StockMovementDto>
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
