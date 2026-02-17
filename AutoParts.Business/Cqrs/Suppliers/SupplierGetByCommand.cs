using System.Net;
using AutoMapper;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DtoModels.Supplier;
using MediatR;

namespace AutoParts.Business.Cqrs.Suppliers;

public class SupplierGetByCommand : IRequest<IDataResult<SupplierDto>>
{
    public int Id { get; }

    public SupplierGetByCommand(int id)
    {
        Id = id;
    }

    public class SupplierGetByCommandHandler : IRequestHandler<SupplierGetByCommand, IDataResult<SupplierDto>>
    {
        #region DI

        private readonly ISupplierDal _supplierDal;
        private readonly IMapper _mapper;

        public SupplierGetByCommandHandler(ISupplierDal supplierDal, IMapper mapper)
        {
            _supplierDal = supplierDal;
            _mapper = mapper;
        }

        #endregion

        public async Task<IDataResult<SupplierDto>> Handle(SupplierGetByCommand request, CancellationToken cancellationToken)
        {
            var source = await _supplierDal.GetAsync(i => i.Id == request.Id);
            if (source == null)
                return new ErrorDataResult<SupplierDto>("Record not found", HttpStatusCode.NotFound);

            return new SuccessDataResult<SupplierDto>(_mapper.Map<SupplierDto>(source));
        }
    }
}
