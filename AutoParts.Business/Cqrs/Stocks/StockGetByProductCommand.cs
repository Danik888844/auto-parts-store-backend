using System.Net;
using AutoMapper;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DtoModels.Stock;
using MediatR;

namespace AutoParts.Business.Cqrs.Stocks;

public class StockGetByProductCommand : IRequest<IDataResult<StockDto>>
{
    public int ProductId { get; }

    public StockGetByProductCommand(int productId)
    {
        ProductId = productId;
    }

    public class StockGetByProductCommandHandler : IRequestHandler<StockGetByProductCommand, IDataResult<StockDto>>
    {
        private readonly IStockDal _dal;
        private readonly IMapper _mapper;

        public StockGetByProductCommandHandler(IStockDal dal, IMapper mapper)
        {
            _dal = dal;
            _mapper = mapper;
        }

        public async Task<IDataResult<StockDto>> Handle(StockGetByProductCommand request, CancellationToken cancellationToken)
        {
            var stock = await _dal.GetByProductIdAsync(request.ProductId);
            if (stock == null)
                return new SuccessDataResult<StockDto>(new StockDto
                {
                    ProductId = request.ProductId,
                    Quantity = 0
                });
            return new SuccessDataResult<StockDto>(_mapper.Map<StockDto>(stock));
        }
    }
}
