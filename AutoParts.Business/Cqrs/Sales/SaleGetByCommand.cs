using System.Net;
using AutoMapper;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DtoModels.Sale;
using MediatR;

namespace AutoParts.Business.Cqrs.Sales;

public class SaleGetByCommand : IRequest<IDataResult<SaleDto>>
{
    public int Id { get; }

    public SaleGetByCommand(int id)
    {
        Id = id;
    }

    public class SaleGetByCommandHandler : IRequestHandler<SaleGetByCommand, IDataResult<SaleDto>>
    {
        private readonly ISaleDal _dal;
        private readonly IMapper _mapper;

        public SaleGetByCommandHandler(ISaleDal dal, IMapper mapper)
        {
            _dal = dal;
            _mapper = mapper;
        }

        public async Task<IDataResult<SaleDto>> Handle(SaleGetByCommand request, CancellationToken cancellationToken)
        {
            var entity = await _dal.GetWithIncludesAsync(request.Id);
            if (entity == null)
                return new ErrorDataResult<SaleDto>("Record not found", HttpStatusCode.NotFound);
            return new SuccessDataResult<SaleDto>(_mapper.Map<SaleDto>(entity));
        }
    }
}
