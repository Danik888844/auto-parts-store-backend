using System.Net;
using AutoMapper;
using AutoParts.Core.GeneralHelpers;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels.Vehicle;
using FluentValidation;
using MediatR;

namespace AutoParts.Business.Cqrs.Vehicles;

public class VehicleCreateCommand : IRequest<IDataResult<object>>
{
    public VehicleFormDto Form { get; }

    public VehicleCreateCommand(VehicleFormDto form)
    {
        Form = form;
    }

    public class VehicleCreateCommandHandler : IRequestHandler<VehicleCreateCommand, IDataResult<object>>
    {
        private readonly IVehicleDal _dal;
        private readonly IValidator<VehicleFormDto> _validator;
        private readonly IXssRepository _xssRepository;
        private readonly IMapper _mapper;

        public VehicleCreateCommandHandler(IVehicleDal dal, IValidator<VehicleFormDto> validator,
            IXssRepository xssRepository, IMapper mapper)
        {
            _dal = dal;
            _validator = validator;
            _xssRepository = xssRepository;
            _mapper = mapper;
        }

        public async Task<IDataResult<object>> Handle(VehicleCreateCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validation.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            var entity = _mapper.Map<Vehicle>(request.Form);
            if (!string.IsNullOrWhiteSpace(entity.Generation))
                entity.Generation = _xssRepository.Execute(entity.Generation);
            if (!string.IsNullOrWhiteSpace(entity.Engine))
                entity.Engine = _xssRepository.Execute(entity.Engine);
            if (!string.IsNullOrWhiteSpace(entity.BodyType))
                entity.BodyType = _xssRepository.Execute(entity.BodyType);
            if (!string.IsNullOrWhiteSpace(entity.Note))
                entity.Note = _xssRepository.Execute(entity.Note);

            await _dal.AddAsync(entity);

            return new SuccessDataResult<object>(_mapper.Map<VehicleDto>(entity), "Vehicle created");
        }
    }
}
