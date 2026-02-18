using System.Net;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using MediatR;

namespace AutoParts.Business.Cqrs.Clients;

public class ClientDeleteCommand : IRequest<IDataResult<object>>
{
    public int Id { get; }

    public ClientDeleteCommand(int id)
    {
        Id = id;
    }

    public class ClientDeleteCommandHandler : IRequestHandler<ClientDeleteCommand, IDataResult<object>>
    {
        private readonly IClientDal _clientDal;

        public ClientDeleteCommandHandler(IClientDal clientDal)
        {
            _clientDal = clientDal;
        }

        public async Task<IDataResult<object>> Handle(ClientDeleteCommand request, CancellationToken cancellationToken)
        {
            var source = await _clientDal.GetAsync(i => i.Id == request.Id);
            if (source == null)
                return new ErrorDataResult<object>("Record not found", HttpStatusCode.NotFound);

            await _clientDal.DeleteAsync(source);

            return new SuccessDataResult<object>("Deleted");
        }
    }
}
