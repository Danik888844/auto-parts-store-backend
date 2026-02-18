using System.Net;
using AutoMapper;
using AutoParts.Core.GeneralHelpers;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels.VehicleModel;
using FluentValidation;
using MediatR;

namespace AutoParts.Business.Cqrs.VehicleModels;

public class VehicleModelCreateCommand : IRequest<IDataResult<object>>
{
    public VehicleModelFormDto Form { get; }

    public VehicleModelCreateCommand(VehicleModelFormDto form)
    {
        Form = form;
    }

    public class VehicleModelCreateCommandHandler : IRequestHandler<VehicleModelCreateCommand, IDataResult<object>>
    {
        private readonly IVehicleModelDal _dal;
        private readonly IValidator<VehicleModelFormDto> _validator;
        private readonly IXssRepository _xssRepository;
        private readonly IMapper _mapper;

        public VehicleModelCreateCommandHandler(IVehicleModelDal dal, IValidator<VehicleModelFormDto> validator,
            IXssRepository xssRepository, IMapper mapper)
        {
            _dal = dal;
            _validator = validator;
            _xssRepository = xssRepository;
            _mapper = mapper;
        }

        public async Task<IDataResult<object>> Handle(VehicleModelCreateCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validation.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            request.Form.Name = _xssRepository.Execute(request.Form.Name);
            var entity = _mapper.Map<VehicleModel>(request.Form);
            await _dal.AddAsync(entity);

            return new SuccessDataResult<object>(_mapper.Map<VehicleModelDto>(entity), $"Created: {entity.Name}");
        }
    }
}
