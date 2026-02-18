using System.Net;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using MediatR;

namespace AutoParts.Business.Cqrs.VehicleModels;

public class VehicleModelDeleteCommand : IRequest<IDataResult<object>>
{
    public int Id { get; }

    public VehicleModelDeleteCommand(int id)
    {
        Id = id;
    }

    public class VehicleModelDeleteCommandHandler : IRequestHandler<VehicleModelDeleteCommand, IDataResult<object>>
    {
        private readonly IVehicleModelDal _dal;

        public VehicleModelDeleteCommandHandler(IVehicleModelDal dal)
        {
            _dal = dal;
        }

        public async Task<IDataResult<object>> Handle(VehicleModelDeleteCommand request, CancellationToken cancellationToken)
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
