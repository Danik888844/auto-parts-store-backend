using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using AutoParts.Core.GeneralHelpers;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels;
using AutoParts.DataAccess.Models.DtoModels.VehicleBrand;
using FluentValidation;
using MediatR;

namespace AutoParts.Business.Cqrs.VehicleBrands;

public class VehicleBrandGetListCommand : IRequest<IDataResult<object>>
{
    public PaginationFormDto Form { get; }

    public VehicleBrandGetListCommand(PaginationFormDto form)
    {
        Form = form;
    }

    public class VehicleBrandGetListCommandHandler : IRequestHandler<VehicleBrandGetListCommand, IDataResult<object>>
    {
        private readonly IVehicleBrandDal _dal;
        private readonly IValidator<PaginationFormDto> _validator;
        private readonly IMapper _mapper;

        public VehicleBrandGetListCommandHandler(IVehicleBrandDal dal, IValidator<PaginationFormDto> validator, IMapper mapper)
        {
            _dal = dal;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<IDataResult<object>> Handle(VehicleBrandGetListCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validation.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            Expression<Func<VehicleBrand, bool>> filter = x => !x.IsDeleted;
            if (!string.IsNullOrWhiteSpace(request.Form.Search))
            {
                var s = request.Form.Search.Trim().ToLower();
                filter = x => !x.IsDeleted && x.Name.ToLower().Contains(s);
            }

            var source = await _dal.GetAllPagedAsync(filter, request.Form.PageNumber, request.Form.ViewSize);
            var items = source.Items.Select(i => _mapper.Map<VehicleBrandDto>(i)).ToList();

            return new SuccessDataResult<object>(new PaginationReturnListDto<VehicleBrandDto>
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
