using System.Net;
using AutoMapper;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DtoModels.Vehicle;
using MediatR;

namespace AutoParts.Business.Cqrs.Vehicles;

public class VehicleGetByCommand : IRequest<IDataResult<VehicleDto>>
{
    public int Id { get; }

    public VehicleGetByCommand(int id)
    {
        Id = id;
    }

    public class VehicleGetByCommandHandler : IRequestHandler<VehicleGetByCommand, IDataResult<VehicleDto>>
    {
        private readonly IVehicleDal _dal;
        private readonly IMapper _mapper;

        public VehicleGetByCommandHandler(IVehicleDal dal, IMapper mapper)
        {
            _dal = dal;
            _mapper = mapper;
        }

        public async Task<IDataResult<VehicleDto>> Handle(VehicleGetByCommand request, CancellationToken cancellationToken)
        {
            var source = await _dal.GetWithIncludesAsync(request.Id);
            if (source == null)
                return new ErrorDataResult<VehicleDto>("Record not found", HttpStatusCode.NotFound);

            return new SuccessDataResult<VehicleDto>(_mapper.Map<VehicleDto>(source));
        }
    }
}
