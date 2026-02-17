using System.Net;
using AutoMapper;
using AutoParts.Core.GeneralHelpers;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DtoModels.Supplier;
using FluentValidation;
using MediatR;

namespace AutoParts.Business.Cqrs.Suppliers;

public class SupplierEditCommand : IRequest<IDataResult<object>>
{
    public int Id { get; }
    public SupplierFormDto Form { get; }

    public SupplierEditCommand(int id, SupplierFormDto form)
    {
        Id = id;
        Form = form;
    }

    public class SupplierEditCommandHandler : IRequestHandler<SupplierEditCommand, IDataResult<object>>
    {
        private readonly IValidator<SupplierFormDto> _validator;
        private readonly ISupplierDal _supplierDal;
        private readonly IXssRepository _xssRepository;
        private readonly IMapper _mapper;

        public SupplierEditCommandHandler(IValidator<SupplierFormDto> validator, ISupplierDal supplierDal,
            IXssRepository xssRepository, IMapper mapper)
        {
            _validator = validator;
            _supplierDal = supplierDal;
            _xssRepository = xssRepository;
            _mapper = mapper;
        }

        public async Task<IDataResult<object>> Handle(SupplierEditCommand request, CancellationToken cancellationToken)
        {
            var validationOfForm = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validationOfForm.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validationOfForm.Errors.Select(e => e.ErrorMessage).ToList());

            var source = await _supplierDal.GetAsync(i => i.Id == request.Id);
            if (source == null)
                return new ErrorDataResult<object>("Record not found", HttpStatusCode.NotFound);

            source.Name = _xssRepository.Execute(request.Form.Name);
            source.Phone = string.IsNullOrWhiteSpace(request.Form.Phone)
                ? null
                : _xssRepository.Execute(request.Form.Phone);
            source.Email = string.IsNullOrWhiteSpace(request.Form.Email)
                ? null
                : _xssRepository.Execute(request.Form.Email);
            source.Address = string.IsNullOrWhiteSpace(request.Form.Address)
                ? null
                : _xssRepository.Execute(request.Form.Address);

            await _supplierDal.UpdateAsync(source);

            var supplierDto = _mapper.Map<SupplierDto>(source);

            return new SuccessDataResult<object>(supplierDto, $"\"{source.Name}\" edited");
        }
    }
}
