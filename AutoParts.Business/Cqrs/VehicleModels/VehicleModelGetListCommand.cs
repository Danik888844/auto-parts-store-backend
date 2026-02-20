using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using AutoParts.Core.GeneralHelpers;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels;
using AutoParts.DataAccess.Models.DtoModels.VehicleModel;
using FluentValidation;
using MediatR;

namespace AutoParts.Business.Cqrs.VehicleModels;

public class VehicleModelGetListCommand : IRequest<IDataResult<object>>
{
    public PaginationFormDto Form { get; }

    public VehicleModelGetListCommand(PaginationFormDto form)
    {
        Form = form;
    }

    public class VehicleModelGetListCommandHandler : IRequestHandler<VehicleModelGetListCommand, IDataResult<object>>
    {
        private readonly IVehicleModelDal _dal;
        private readonly IValidator<PaginationFormDto> _validator;
        private readonly IMapper _mapper;

        public VehicleModelGetListCommandHandler(IVehicleModelDal dal, IValidator<PaginationFormDto> validator, IMapper mapper)
        {
            _dal = dal;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<IDataResult<object>> Handle(VehicleModelGetListCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validation.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            Expression<Func<VehicleModel, bool>>? filter = null;
            if (!string.IsNullOrWhiteSpace(request.Form.Search))
            {
                var s = request.Form.Search.Trim().ToLower();
                filter = m => m.Name.ToLower().Contains(s) || (m.Brand != null && m.Brand.Name.ToLower().Contains(s));
            }

            var source = await _dal.GetPagedWithIncludesAsync(filter, request.Form.PageNumber, request.Form.ViewSize);
            var items = source.Items.Select(i => _mapper.Map<VehicleModelDto>(i)).ToList();

            return new SuccessDataResult<object>(new PaginationReturnListDto<VehicleModelDto>
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
