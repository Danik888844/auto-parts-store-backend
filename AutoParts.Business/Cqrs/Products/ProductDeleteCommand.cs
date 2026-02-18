using System.Net;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using MediatR;

namespace AutoParts.Business.Cqrs.Products;

public class ProductDeleteCommand : IRequest<IDataResult<object>>
{
    public int Id { get; }

    public ProductDeleteCommand(int id)
    {
        Id = id;
    }

    public class ProductDeleteCommandHandler : IRequestHandler<ProductDeleteCommand, IDataResult<object>>
    {
        private readonly IProductDal _productDal;

        public ProductDeleteCommandHandler(IProductDal productDal)
        {
            _productDal = productDal;
        }

        public async Task<IDataResult<object>> Handle(ProductDeleteCommand request, CancellationToken cancellationToken)
        {
            var source = await _productDal.GetAsync(p => p.Id == request.Id && !p.IsDeleted);
            if (source == null)
                return new ErrorDataResult<object>("Record not found", HttpStatusCode.NotFound);

            source.IsDeleted = true;
            await _productDal.UpdateAsync(source);

            return new SuccessDataResult<object>("Deleted");
        }
    }
}
