using System.Net;
using AutoMapper;
using AutoParts.Core.GeneralHelpers;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DtoModels.Client;
using FluentValidation;
using MediatR;

namespace AutoParts.Business.Cqrs.Clients;

public class ClientEditCommand : IRequest<IDataResult<object>>
{
    public int Id { get; }
    public ClientFormDto Form { get; }

    public ClientEditCommand(int id, ClientFormDto form)
    {
        Id = id;
        Form = form;
    }

    public class ClientEditCommandHandler : IRequestHandler<ClientEditCommand, IDataResult<object>>
    {
        private readonly IValidator<ClientFormDto> _validator;
        private readonly IClientDal _clientDal;
        private readonly IXssRepository _xssRepository;
        private readonly IMapper _mapper;

        public ClientEditCommandHandler(IValidator<ClientFormDto> validator, IClientDal clientDal,
            IXssRepository xssRepository, IMapper mapper)
        {
            _validator = validator;
            _clientDal = clientDal;
            _xssRepository = xssRepository;
            _mapper = mapper;
        }

        public async Task<IDataResult<object>> Handle(ClientEditCommand request, CancellationToken cancellationToken)
        {
            var validationOfForm = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validationOfForm.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validationOfForm.Errors.Select(e => e.ErrorMessage).ToList());

            var source = await _clientDal.GetAsync(i => i.Id == request.Id);
            if (source == null)
                return new ErrorDataResult<object>("Record not found", HttpStatusCode.NotFound);

            source.FullName = _xssRepository.Execute(request.Form.FullName);
            source.Phone = string.IsNullOrWhiteSpace(request.Form.Phone)
                ? null
                : _xssRepository.Execute(request.Form.Phone);
            source.Email = string.IsNullOrWhiteSpace(request.Form.Email)
                ? null
                : _xssRepository.Execute(request.Form.Email);
            source.Notes = string.IsNullOrWhiteSpace(request.Form.Notes)
                ? null
                : _xssRepository.Execute(request.Form.Notes);

            await _clientDal.UpdateAsync(source);

            var clientDto = _mapper.Map<ClientDto>(source);

            return new SuccessDataResult<object>(clientDto, $"\"{source.FullName}\" edited");
        }
    }
}
