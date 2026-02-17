using System.Net;
using AutoMapper;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DtoModels.Manufacturer;
using MediatR;

namespace AutoParts.Business.Cqrs.Manufacturers;

public class ManufacturerGetByCommand : IRequest<IDataResult<ManufacturerDto>>
{
    public int Id { get; }

    public ManufacturerGetByCommand(int id)
    {
        Id = id;
    }

    public class ManufacturerGetByCommandHandler : IRequestHandler<ManufacturerGetByCommand, IDataResult<ManufacturerDto>>
    {
        #region DI

        private readonly IManufacturerDal _manufacturerDal;
        private readonly IMapper _mapper;

        public ManufacturerGetByCommandHandler(IManufacturerDal manufacturerDal, IMapper mapper)
        {
            _manufacturerDal = manufacturerDal;
            _mapper = mapper;
        }

        #endregion

        public async Task<IDataResult<ManufacturerDto>> Handle(ManufacturerGetByCommand request, CancellationToken cancellationToken)
        {
            var source = await _manufacturerDal.GetAsync(i => i.Id == request.Id);
            if (source == null)
                return new ErrorDataResult<ManufacturerDto>("Record not found", HttpStatusCode.NotFound);

            return new SuccessDataResult<ManufacturerDto>(_mapper.Map<ManufacturerDto>(source));
        }
    }
}
