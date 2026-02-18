using System.Net;
using AutoMapper;
using AutoParts.Core.GeneralHelpers;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DtoModels.VehicleBrand;
using FluentValidation;
using MediatR;

namespace AutoParts.Business.Cqrs.VehicleBrands;

public class VehicleBrandEditCommand : IRequest<IDataResult<object>>
{
    public int Id { get; }
    public VehicleBrandFormDto Form { get; }

    public VehicleBrandEditCommand(int id, VehicleBrandFormDto form)
    {
        Id = id;
        Form = form;
    }

    public class VehicleBrandEditCommandHandler : IRequestHandler<VehicleBrandEditCommand, IDataResult<object>>
    {
        private readonly IVehicleBrandDal _dal;
        private readonly IValidator<VehicleBrandFormDto> _validator;
        private readonly IXssRepository _xssRepository;
        private readonly IMapper _mapper;

        public VehicleBrandEditCommandHandler(IVehicleBrandDal dal, IValidator<VehicleBrandFormDto> validator,
            IXssRepository xssRepository, IMapper mapper)
        {
            _dal = dal;
            _validator = validator;
            _xssRepository = xssRepository;
            _mapper = mapper;
        }

        public async Task<IDataResult<object>> Handle(VehicleBrandEditCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validation.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            var source = await _dal.GetAsync(x => x.Id == request.Id && !x.IsDeleted);
            if (source == null)
                return new ErrorDataResult<object>("Record not found", HttpStatusCode.NotFound);

            source.Name = _xssRepository.Execute(request.Form.Name);
            await _dal.UpdateAsync(source);

            return new SuccessDataResult<object>(_mapper.Map<VehicleBrandDto>(source), $"\"{source.Name}\" edited");
        }
    }
}
