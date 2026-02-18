using System.Net;
using AutoMapper;
using AutoParts.Core.GeneralHelpers;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels.VehicleBrand;
using FluentValidation;
using MediatR;

namespace AutoParts.Business.Cqrs.VehicleBrands;

public class VehicleBrandCreateCommand : IRequest<IDataResult<object>>
{
    public VehicleBrandFormDto Form { get; }

    public VehicleBrandCreateCommand(VehicleBrandFormDto form)
    {
        Form = form;
    }

    public class VehicleBrandCreateCommandHandler : IRequestHandler<VehicleBrandCreateCommand, IDataResult<object>>
    {
        private readonly IVehicleBrandDal _dal;
        private readonly IValidator<VehicleBrandFormDto> _validator;
        private readonly IXssRepository _xssRepository;
        private readonly IMapper _mapper;

        public VehicleBrandCreateCommandHandler(IVehicleBrandDal dal, IValidator<VehicleBrandFormDto> validator,
            IXssRepository xssRepository, IMapper mapper)
        {
            _dal = dal;
            _validator = validator;
            _xssRepository = xssRepository;
            _mapper = mapper;
        }

        public async Task<IDataResult<object>> Handle(VehicleBrandCreateCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validation.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            request.Form.Name = _xssRepository.Execute(request.Form.Name);
            var entity = _mapper.Map<VehicleBrand>(request.Form);
            await _dal.AddAsync(entity);

            return new SuccessDataResult<object>(_mapper.Map<VehicleBrandDto>(entity), $"Created: {entity.Name}");
        }
    }
}
