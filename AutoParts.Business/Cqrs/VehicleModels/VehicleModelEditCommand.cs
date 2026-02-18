using System.Net;
using AutoMapper;
using AutoParts.Core.GeneralHelpers;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DtoModels.VehicleModel;
using FluentValidation;
using MediatR;

namespace AutoParts.Business.Cqrs.VehicleModels;

public class VehicleModelEditCommand : IRequest<IDataResult<object>>
{
    public int Id { get; }
    public VehicleModelFormDto Form { get; }

    public VehicleModelEditCommand(int id, VehicleModelFormDto form)
    {
        Id = id;
        Form = form;
    }

    public class VehicleModelEditCommandHandler : IRequestHandler<VehicleModelEditCommand, IDataResult<object>>
    {
        private readonly IVehicleModelDal _dal;
        private readonly IValidator<VehicleModelFormDto> _validator;
        private readonly IXssRepository _xssRepository;
        private readonly IMapper _mapper;

        public VehicleModelEditCommandHandler(IVehicleModelDal dal, IValidator<VehicleModelFormDto> validator,
            IXssRepository xssRepository, IMapper mapper)
        {
            _dal = dal;
            _validator = validator;
            _xssRepository = xssRepository;
            _mapper = mapper;
        }

        public async Task<IDataResult<object>> Handle(VehicleModelEditCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validation.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            var source = await _dal.GetAsync(x => x.Id == request.Id && !x.IsDeleted);
            if (source == null)
                return new ErrorDataResult<object>("Record not found", HttpStatusCode.NotFound);

            source.BrandId = request.Form.BrandId;
            source.Name = _xssRepository.Execute(request.Form.Name);
            await _dal.UpdateAsync(source);

            return new SuccessDataResult<object>(_mapper.Map<VehicleModelDto>(source), $"\"{source.Name}\" edited");
        }
    }
}
