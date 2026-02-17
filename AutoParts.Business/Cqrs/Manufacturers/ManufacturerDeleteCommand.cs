using System.Net;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using MediatR;

namespace AutoParts.Business.Cqrs.Manufacturers;

public class ManufacturerDeleteCommand : IRequest<IDataResult<object>>
{
    public int Id { get; }

    public ManufacturerDeleteCommand(int id)
    {
        Id = id;
    }

    public class ManufacturerDeleteCommandHandler : IRequestHandler<ManufacturerDeleteCommand, IDataResult<object>>
    {
        private readonly IManufacturerDal _manufacturerDal;

        public ManufacturerDeleteCommandHandler(IManufacturerDal manufacturerDal)
        {
            _manufacturerDal = manufacturerDal;
        }

        public async Task<IDataResult<object>> Handle(ManufacturerDeleteCommand request, CancellationToken cancellationToken)
        {
            var source = await _manufacturerDal.GetAsync(i => i.Id == request.Id);
            if (source == null)
                return new ErrorDataResult<object>("Record not found", HttpStatusCode.NotFound);

            await _manufacturerDal.DeleteAsync(source);

            return new SuccessDataResult<object>("Deleted");
        }
    }
}
