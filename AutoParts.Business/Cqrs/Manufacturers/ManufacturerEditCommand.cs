using System.Net;
using AutoMapper;
using AutoParts.Core.GeneralHelpers;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DtoModels.Manufacturer;
using FluentValidation;
using MediatR;

namespace AutoParts.Business.Cqrs.Manufacturers;

public class ManufacturerEditCommand : IRequest<IDataResult<object>>
{
    public int Id { get; }
    public ManufacturerFormDto Form { get; }

    public ManufacturerEditCommand(int id, ManufacturerFormDto form)
    {
        Id = id;
        Form = form;
    }

    public class ManufacturerEditCommandHandler : IRequestHandler<ManufacturerEditCommand, IDataResult<object>>
    {
        private readonly IValidator<ManufacturerFormDto> _validator;
        private readonly IManufacturerDal _manufacturerDal;
        private readonly IXssRepository _xssRepository;
        private readonly IMapper _mapper;

        public ManufacturerEditCommandHandler(IValidator<ManufacturerFormDto> validator, IManufacturerDal manufacturerDal,
            IXssRepository xssRepository, IMapper mapper)
        {
            _validator = validator;
            _manufacturerDal = manufacturerDal;
            _xssRepository = xssRepository;
            _mapper = mapper;
        }

        public async Task<IDataResult<object>> Handle(ManufacturerEditCommand request, CancellationToken cancellationToken)
        {
            var validationOfForm = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validationOfForm.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validationOfForm.Errors.Select(e => e.ErrorMessage).ToList());

            var source = await _manufacturerDal.GetAsync(i => i.Id == request.Id);
            if (source == null)
                return new ErrorDataResult<object>("Record not found", HttpStatusCode.NotFound);

            source.Name = _xssRepository.Execute(request.Form.Name);
            source.Country = string.IsNullOrWhiteSpace(request.Form.Country)
                ? null
                : _xssRepository.Execute(request.Form.Country);

            await _manufacturerDal.UpdateAsync(source);

            var manufacturerDto = _mapper.Map<ManufacturerDto>(source);

            return new SuccessDataResult<object>(manufacturerDto, $"\"{source.Name}\" edited");
        }
    }
}
