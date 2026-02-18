using System.Net;
using AutoMapper;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DtoModels.VehicleBrand;
using MediatR;

namespace AutoParts.Business.Cqrs.VehicleBrands;

public class VehicleBrandGetByCommand : IRequest<IDataResult<VehicleBrandDto>>
{
    public int Id { get; }

    public VehicleBrandGetByCommand(int id)
    {
        Id = id;
    }

    public class VehicleBrandGetByCommandHandler : IRequestHandler<VehicleBrandGetByCommand, IDataResult<VehicleBrandDto>>
    {
        private readonly IVehicleBrandDal _dal;
        private readonly IMapper _mapper;

        public VehicleBrandGetByCommandHandler(IVehicleBrandDal dal, IMapper mapper)
        {
            _dal = dal;
            _mapper = mapper;
        }

        public async Task<IDataResult<VehicleBrandDto>> Handle(VehicleBrandGetByCommand request, CancellationToken cancellationToken)
        {
            var source = await _dal.GetAsync(x => x.Id == request.Id && !x.IsDeleted);
            if (source == null)
                return new ErrorDataResult<VehicleBrandDto>("Record not found", HttpStatusCode.NotFound);

            return new SuccessDataResult<VehicleBrandDto>(_mapper.Map<VehicleBrandDto>(source));
        }
    }
}
