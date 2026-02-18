using System.Net;
using AutoMapper;
using AutoParts.Core.GeneralHelpers;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels.Client;
using FluentValidation;
using MediatR;

namespace AutoParts.Business.Cqrs.Clients;

public class ClientCreateCommand : IRequest<IDataResult<object>>
{
    public ClientFormDto Form { get; }

    public ClientCreateCommand(ClientFormDto form)
    {
        Form = form;
    }

    public class ClientCreateCommandHandler : IRequestHandler<ClientCreateCommand, IDataResult<object>>
    {
        #region DI

        private readonly IClientDal _clientDal;
        private readonly IValidator<ClientFormDto> _validator;
        private readonly IXssRepository _xssRepository;
        private readonly IMapper _mapper;

        public ClientCreateCommandHandler(IClientDal clientDal, IValidator<ClientFormDto> validator,
            IXssRepository xssRepository, IMapper mapper)
        {
            _clientDal = clientDal;
            _validator = validator;
            _xssRepository = xssRepository;
            _mapper = mapper;
        }

        #endregion

        public async Task<IDataResult<object>> Handle(ClientCreateCommand request, CancellationToken cancellationToken)
        {
            var validationOfForm = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validationOfForm.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validationOfForm.Errors.Select(e => e.ErrorMessage).ToList());

            request.Form.FullName = _xssRepository.Execute(request.Form.FullName);
            if (!string.IsNullOrWhiteSpace(request.Form.Phone))
                request.Form.Phone = _xssRepository.Execute(request.Form.Phone);
            if (!string.IsNullOrWhiteSpace(request.Form.Email))
                request.Form.Email = _xssRepository.Execute(request.Form.Email);
            if (!string.IsNullOrWhiteSpace(request.Form.Notes))
                request.Form.Notes = _xssRepository.Execute(request.Form.Notes);

            var source = _mapper.Map<Client>(request.Form);

            await _clientDal.AddAsync(source);

            var clientDto = _mapper.Map<ClientDto>(source);

            return new SuccessDataResult<object>(clientDto, $"Created: {source.FullName}");
        }
    }
}
