using System.Net;
using AutoMapper;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DtoModels.ProductCompatibility;
using MediatR;

namespace AutoParts.Business.Cqrs.ProductCompatibilities;

public class ProductCompatibilityGetByCommand : IRequest<IDataResult<ProductCompatibilityDto>>
{
    public int Id { get; }

    public ProductCompatibilityGetByCommand(int id)
    {
        Id = id;
    }

    public class ProductCompatibilityGetByCommandHandler : IRequestHandler<ProductCompatibilityGetByCommand, IDataResult<ProductCompatibilityDto>>
    {
        private readonly IProductCompatibilityDal _compatibilityDal;
        private readonly IMapper _mapper;

        public ProductCompatibilityGetByCommandHandler(IProductCompatibilityDal compatibilityDal, IMapper mapper)
        {
            _compatibilityDal = compatibilityDal;
            _mapper = mapper;
        }

        public async Task<IDataResult<ProductCompatibilityDto>> Handle(ProductCompatibilityGetByCommand request, CancellationToken cancellationToken)
        {
            var source = await _compatibilityDal.GetWithIncludesAsync(request.Id);
            if (source == null)
                return new ErrorDataResult<ProductCompatibilityDto>("Record not found", HttpStatusCode.NotFound);

            return new SuccessDataResult<ProductCompatibilityDto>(_mapper.Map<ProductCompatibilityDto>(source));
        }
    }
}
