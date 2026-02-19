using System.Net;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using MediatR;

namespace AutoParts.Business.Cqrs.ProductCompatibilities;

public class ProductCompatibilityDeleteCommand : IRequest<IDataResult<object>>
{
    public int Id { get; }

    public ProductCompatibilityDeleteCommand(int id)
    {
        Id = id;
    }

    public class ProductCompatibilityDeleteCommandHandler : IRequestHandler<ProductCompatibilityDeleteCommand, IDataResult<object>>
    {
        private readonly IProductCompatibilityDal _compatibilityDal;

        public ProductCompatibilityDeleteCommandHandler(IProductCompatibilityDal compatibilityDal)
        {
            _compatibilityDal = compatibilityDal;
        }

        public async Task<IDataResult<object>> Handle(ProductCompatibilityDeleteCommand request, CancellationToken cancellationToken)
        {
            var source = await _compatibilityDal.GetAsync(pc => pc.Id == request.Id && !pc.IsDeleted);
            if (source == null)
                return new ErrorDataResult<object>("Record not found", HttpStatusCode.NotFound);

            source.IsDeleted = true;
            await _compatibilityDal.UpdateAsync(source);

            return new SuccessDataResult<object>("Deleted");
        }
    }
}
