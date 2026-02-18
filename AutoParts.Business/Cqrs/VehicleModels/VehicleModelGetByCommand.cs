using System.Net;
using AutoMapper;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DtoModels.VehicleModel;
using MediatR;

namespace AutoParts.Business.Cqrs.VehicleModels;

public class VehicleModelGetByCommand : IRequest<IDataResult<VehicleModelDto>>
{
    public int Id { get; }

    public VehicleModelGetByCommand(int id)
    {
        Id = id;
    }

    public class VehicleModelGetByCommandHandler : IRequestHandler<VehicleModelGetByCommand, IDataResult<VehicleModelDto>>
    {
        private readonly IVehicleModelDal _dal;
        private readonly IMapper _mapper;

        public VehicleModelGetByCommandHandler(IVehicleModelDal dal, IMapper mapper)
        {
            _dal = dal;
            _mapper = mapper;
        }

        public async Task<IDataResult<VehicleModelDto>> Handle(VehicleModelGetByCommand request, CancellationToken cancellationToken)
        {
            var source = await _dal.GetWithIncludesAsync(request.Id);
            if (source == null)
                return new ErrorDataResult<VehicleModelDto>("Record not found", HttpStatusCode.NotFound);

            return new SuccessDataResult<VehicleModelDto>(_mapper.Map<VehicleModelDto>(source));
        }
    }
}
