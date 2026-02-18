using System.Net;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using MediatR;

namespace AutoParts.Business.Cqrs.Vehicles;

public class VehicleDeleteCommand : IRequest<IDataResult<object>>
{
    public int Id { get; }

    public VehicleDeleteCommand(int id)
    {
        Id = id;
    }

    public class VehicleDeleteCommandHandler : IRequestHandler<VehicleDeleteCommand, IDataResult<object>>
    {
        private readonly IVehicleDal _dal;

        public VehicleDeleteCommandHandler(IVehicleDal dal)
        {
            _dal = dal;
        }

        public async Task<IDataResult<object>> Handle(VehicleDeleteCommand request, CancellationToken cancellationToken)
        {
            var source = await _dal.GetAsync(x => x.Id == request.Id && !x.IsDeleted);
            if (source == null)
                return new ErrorDataResult<object>("Record not found", HttpStatusCode.NotFound);

            source.IsDeleted = true;
            await _dal.UpdateAsync(source);

            return new SuccessDataResult<object>("Deleted");
        }
    }
}
