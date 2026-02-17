using System.Net;
using AutoMapper;
using AutoParts.Core.GeneralHelpers;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels.Supplier;
using FluentValidation;
using MediatR;

namespace AutoParts.Business.Cqrs.Suppliers;

public class SupplierCreateCommand : IRequest<IDataResult<object>>
{
    public SupplierFormDto Form { get; }

    public SupplierCreateCommand(SupplierFormDto form)
    {
        Form = form;
    }

    public class SupplierCreateCommandHandler : IRequestHandler<SupplierCreateCommand, IDataResult<object>>
    {
        #region DI

        private readonly ISupplierDal _supplierDal;
        private readonly IValidator<SupplierFormDto> _validator;
        private readonly IXssRepository _xssRepository;
        private readonly IMapper _mapper;

        public SupplierCreateCommandHandler(ISupplierDal supplierDal, IValidator<SupplierFormDto> validator,
            IXssRepository xssRepository, IMapper mapper)
        {
            _supplierDal = supplierDal;
            _validator = validator;
            _xssRepository = xssRepository;
            _mapper = mapper;
        }

        #endregion

        public async Task<IDataResult<object>> Handle(SupplierCreateCommand request, CancellationToken cancellationToken)
        {
            var validationOfForm = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validationOfForm.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validationOfForm.Errors.Select(e => e.ErrorMessage).ToList());

            request.Form.Name = _xssRepository.Execute(request.Form.Name);
            if (!string.IsNullOrWhiteSpace(request.Form.Phone))
                request.Form.Phone = _xssRepository.Execute(request.Form.Phone);
            if (!string.IsNullOrWhiteSpace(request.Form.Email))
                request.Form.Email = _xssRepository.Execute(request.Form.Email);
            if (!string.IsNullOrWhiteSpace(request.Form.Address))
                request.Form.Address = _xssRepository.Execute(request.Form.Address);

            var source = _mapper.Map<Supplier>(request.Form);

            await _supplierDal.AddAsync(source);

            var supplierDto = _mapper.Map<SupplierDto>(source);

            return new SuccessDataResult<object>(supplierDto, $"Created: {source.Name}");
        }
    }
}
