using System.Net;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DtoModels.Product;
using MediatR;

namespace AutoParts.Business.Cqrs.Products;

public class ProductCheckSkuCommand : IRequest<IDataResult<ProductCheckSkuResultDto>>
{
    public string Sku { get; }
    public int? ExcludeId { get; }

    public ProductCheckSkuCommand(string sku, int? excludeId = null)
    {
        Sku = sku;
        ExcludeId = excludeId;
    }

    public class ProductCheckSkuCommandHandler : IRequestHandler<ProductCheckSkuCommand, IDataResult<ProductCheckSkuResultDto>>
    {
        private readonly IProductDal _productDal;

        public ProductCheckSkuCommandHandler(IProductDal productDal)
        {
            _productDal = productDal;
        }

        public async Task<IDataResult<ProductCheckSkuResultDto>> Handle(ProductCheckSkuCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Sku))
                return new ErrorDataResult<ProductCheckSkuResultDto>("SKU is required", HttpStatusCode.BadRequest);

            var exists = await _productDal.AnyAsync(p =>
                p.Sku.Trim().ToLower() == request.Sku.Trim().ToLower()
                && !p.IsDeleted
                && (request.ExcludeId == null || p.Id != request.ExcludeId.Value));

            return new SuccessDataResult<ProductCheckSkuResultDto>(
                new ProductCheckSkuResultDto { Exists = exists });
        }
    }
}
