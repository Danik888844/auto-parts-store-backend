using System.Net;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using MediatR;

namespace AutoParts.Business.Cqrs.Suppliers;

public class SupplierDeleteCommand : IRequest<IDataResult<object>>
{
    public int Id { get; }

    public SupplierDeleteCommand(int id)
    {
        Id = id;
    }

    public class SupplierDeleteCommandHandler : IRequestHandler<SupplierDeleteCommand, IDataResult<object>>
    {
        private readonly ISupplierDal _supplierDal;

        public SupplierDeleteCommandHandler(ISupplierDal supplierDal)
        {
            _supplierDal = supplierDal;
        }

        public async Task<IDataResult<object>> Handle(SupplierDeleteCommand request, CancellationToken cancellationToken)
        {
            var source = await _supplierDal.GetAsync(i => i.Id == request.Id);
            if (source == null)
                return new ErrorDataResult<object>("Record not found", HttpStatusCode.NotFound);

            await _supplierDal.DeleteAsync(source);

            return new SuccessDataResult<object>("Deleted");
        }
    }
}
