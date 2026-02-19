using System.Net;
using AutoMapper;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DtoModels.StockMovement;
using MediatR;

namespace AutoParts.Business.Cqrs.StockMovements;

public class StockMovementGetByCommand : IRequest<IDataResult<StockMovementDto>>
{
    public int Id { get; }

    public StockMovementGetByCommand(int id)
    {
        Id = id;
    }

    public class StockMovementGetByCommandHandler : IRequestHandler<StockMovementGetByCommand, IDataResult<StockMovementDto>>
    {
        private readonly IStockMovementDal _dal;
        private readonly IMapper _mapper;

        public StockMovementGetByCommandHandler(IStockMovementDal dal, IMapper mapper)
        {
            _dal = dal;
            _mapper = mapper;
        }

        public async Task<IDataResult<StockMovementDto>> Handle(StockMovementGetByCommand request, CancellationToken cancellationToken)
        {
            var entity = await _dal.GetWithIncludesAsync(request.Id);
            if (entity == null)
                return new ErrorDataResult<StockMovementDto>("Record not found", HttpStatusCode.NotFound);
            return new SuccessDataResult<StockMovementDto>(_mapper.Map<StockMovementDto>(entity));
        }
    }
}
