using System.Net;
using AutoMapper;
using AutoParts.Core.GeneralHelpers;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels.Manufacturer;
using FluentValidation;
using MediatR;

namespace AutoParts.Business.Cqrs.Manufacturers;

public class ManufacturerCreateCommand : IRequest<IDataResult<object>>
{
    public ManufacturerFormDto Form { get; }

    public ManufacturerCreateCommand(ManufacturerFormDto form)
    {
        Form = form;
    }

    public class ManufacturerCreateCommandHandler : IRequestHandler<ManufacturerCreateCommand, IDataResult<object>>
    {
        #region DI

        private readonly IManufacturerDal _manufacturerDal;
        private readonly IValidator<ManufacturerFormDto> _validator;
        private readonly IXssRepository _xssRepository;
        private readonly IMapper _mapper;

        public ManufacturerCreateCommandHandler(IManufacturerDal manufacturerDal, IValidator<ManufacturerFormDto> validator,
            IXssRepository xssRepository, IMapper mapper)
        {
            _manufacturerDal = manufacturerDal;
            _validator = validator;
            _xssRepository = xssRepository;
            _mapper = mapper;
        }

        #endregion

        public async Task<IDataResult<object>> Handle(ManufacturerCreateCommand request, CancellationToken cancellationToken)
        {
            var validationOfForm = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validationOfForm.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validationOfForm.Errors.Select(e => e.ErrorMessage).ToList());

            request.Form.Name = _xssRepository.Execute(request.Form.Name);
            if (!string.IsNullOrWhiteSpace(request.Form.Country))
                request.Form.Country = _xssRepository.Execute(request.Form.Country);

            var source = _mapper.Map<Manufacturer>(request.Form);

            await _manufacturerDal.AddAsync(source);

            var manufacturerDto = _mapper.Map<ManufacturerDto>(source);

            return new SuccessDataResult<object>(manufacturerDto, $"Created: {source.Name}");
        }
    }
}
