using System.Net;
using AutoMapper;
using AutoParts.Core.GeneralHelpers;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DtoModels.Vehicle;
using FluentValidation;
using MediatR;

namespace AutoParts.Business.Cqrs.Vehicles;

public class VehicleEditCommand : IRequest<IDataResult<object>>
{
    public int Id { get; }
    public VehicleFormDto Form { get; }

    public VehicleEditCommand(int id, VehicleFormDto form)
    {
        Id = id;
        Form = form;
    }

    public class VehicleEditCommandHandler : IRequestHandler<VehicleEditCommand, IDataResult<object>>
    {
        private readonly IVehicleDal _dal;
        private readonly IValidator<VehicleFormDto> _validator;
        private readonly IXssRepository _xssRepository;
        private readonly IMapper _mapper;

        public VehicleEditCommandHandler(IVehicleDal dal, IValidator<VehicleFormDto> validator,
            IXssRepository xssRepository, IMapper mapper)
        {
            _dal = dal;
            _validator = validator;
            _xssRepository = xssRepository;
            _mapper = mapper;
        }

        public async Task<IDataResult<object>> Handle(VehicleEditCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validation.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            var source = await _dal.GetAsync(x => x.Id == request.Id && !x.IsDeleted);
            if (source == null)
                return new ErrorDataResult<object>("Record not found", HttpStatusCode.NotFound);

            source.ModelId = request.Form.ModelId;
            source.YearFrom = request.Form.YearFrom;
            source.YearTo = request.Form.YearTo;
            source.Generation = string.IsNullOrWhiteSpace(request.Form.Generation) ? null : _xssRepository.Execute(request.Form.Generation);
            source.Engine = string.IsNullOrWhiteSpace(request.Form.Engine) ? null : _xssRepository.Execute(request.Form.Engine);
            source.BodyType = string.IsNullOrWhiteSpace(request.Form.BodyType) ? null : _xssRepository.Execute(request.Form.BodyType);
            source.Note = string.IsNullOrWhiteSpace(request.Form.Note) ? null : _xssRepository.Execute(request.Form.Note);

            await _dal.UpdateAsync(source);

            return new SuccessDataResult<object>(_mapper.Map<VehicleDto>(source), "Vehicle edited");
        }
    }
}
