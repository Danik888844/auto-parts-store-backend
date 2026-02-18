using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels;
using AutoParts.DataAccess.Models.DtoModels.Vehicle;
using FluentValidation;
using MediatR;

namespace AutoParts.Business.Cqrs.Vehicles;

public class VehicleGetListCommand : IRequest<IDataResult<object>>
{
    public PaginationFormDto Form { get; }

    public VehicleGetListCommand(PaginationFormDto form)
    {
        Form = form;
    }

    public class VehicleGetListCommandHandler : IRequestHandler<VehicleGetListCommand, IDataResult<object>>
    {
        private readonly IVehicleDal _dal;
        private readonly IValidator<PaginationFormDto> _validator;
        private readonly IMapper _mapper;

        public VehicleGetListCommandHandler(IVehicleDal dal, IValidator<PaginationFormDto> validator, IMapper mapper)
        {
            _dal = dal;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<IDataResult<object>> Handle(VehicleGetListCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validation.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            Expression<Func<Vehicle, bool>>? filter = null;
            if (!string.IsNullOrWhiteSpace(request.Form.Search))
            {
                var s = request.Form.Search.Trim().ToLower();
                filter = v => (v.Model != null && v.Model.Name.ToLower().Contains(s))
                    || (v.Model != null && v.Model.Brand != null && v.Model.Brand.Name.ToLower().Contains(s))
                    || (v.Generation != null && v.Generation.ToLower().Contains(s))
                    || (v.Engine != null && v.Engine.ToLower().Contains(s))
                    || (v.BodyType != null && v.BodyType.ToLower().Contains(s));
            }

            var source = await _dal.GetPagedWithIncludesAsync(filter, request.Form.PageNumber, request.Form.ViewSize);
            var items = source.Items.Select(i => _mapper.Map<VehicleDto>(i)).ToList();

            return new SuccessDataResult<object>(new PaginationReturnListDto<VehicleDto>
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
