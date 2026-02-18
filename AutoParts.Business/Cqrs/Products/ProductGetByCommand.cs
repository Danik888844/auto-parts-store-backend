using System.Net;
using AutoMapper;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DtoModels.Product;
using MediatR;

namespace AutoParts.Business.Cqrs.Products;

public class ProductGetByCommand : IRequest<IDataResult<ProductDto>>
{
    public int Id { get; }

    public ProductGetByCommand(int id)
    {
        Id = id;
    }

    public class ProductGetByCommandHandler : IRequestHandler<ProductGetByCommand, IDataResult<ProductDto>>
    {
        private readonly IProductDal _productDal;
        private readonly IMapper _mapper;

        public ProductGetByCommandHandler(IProductDal productDal, IMapper mapper)
        {
            _productDal = productDal;
            _mapper = mapper;
        }

        public async Task<IDataResult<ProductDto>> Handle(ProductGetByCommand request, CancellationToken cancellationToken)
        {
            var source = await _productDal.GetWithIncludesAsync(request.Id);
            if (source == null)
                return new ErrorDataResult<ProductDto>("Record not found", HttpStatusCode.NotFound);

            return new SuccessDataResult<ProductDto>(_mapper.Map<ProductDto>(source));
        }
    }
}
