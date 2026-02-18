using System.Net;
using AutoMapper;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DtoModels.Client;
using MediatR;

namespace AutoParts.Business.Cqrs.Clients;

public class ClientGetByCommand : IRequest<IDataResult<ClientDto>>
{
    public int Id { get; }

    public ClientGetByCommand(int id)
    {
        Id = id;
    }

    public class ClientGetByCommandHandler : IRequestHandler<ClientGetByCommand, IDataResult<ClientDto>>
    {
        #region DI

        private readonly IClientDal _clientDal;
        private readonly IMapper _mapper;

        public ClientGetByCommandHandler(IClientDal clientDal, IMapper mapper)
        {
            _clientDal = clientDal;
            _mapper = mapper;
        }

        #endregion

        public async Task<IDataResult<ClientDto>> Handle(ClientGetByCommand request, CancellationToken cancellationToken)
        {
            var source = await _clientDal.GetAsync(i => i.Id == request.Id && !i.IsDeleted);
            if (source == null)
                return new ErrorDataResult<ClientDto>("Record not found", HttpStatusCode.NotFound);

            return new SuccessDataResult<ClientDto>(_mapper.Map<ClientDto>(source));
        }
    }
}
