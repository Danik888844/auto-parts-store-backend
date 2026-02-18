using System.Net;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using MediatR;

namespace AutoParts.Business.Cqrs.VehicleBrands;

public class VehicleBrandDeleteCommand : IRequest<IDataResult<object>>
{
    public int Id { get; }

    public VehicleBrandDeleteCommand(int id)
    {
        Id = id;
    }

    public class VehicleBrandDeleteCommandHandler : IRequestHandler<VehicleBrandDeleteCommand, IDataResult<object>>
    {
        private readonly IVehicleBrandDal _dal;

        public VehicleBrandDeleteCommandHandler(IVehicleBrandDal dal)
        {
            _dal = dal;
        }

        public async Task<IDataResult<object>> Handle(VehicleBrandDeleteCommand request, CancellationToken cancellationToken)
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
