using System.Net;
using AutoMapper;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Contexts;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels.Sale;
using AutoParts.DataAccess.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoParts.Business.Cqrs.Sales;

/// <summary>
/// Отменяет продажу в статусе Draft. Остатки не затрагиваются (черновик их не резервировал).
/// </summary>
public class CancelSaleCommand : IRequest<IDataResult<object>>
{
    public int SaleId { get; }

    public CancelSaleCommand(int saleId)
    {
        SaleId = saleId;
    }

    public class CancelSaleCommandHandler : IRequestHandler<CancelSaleCommand, IDataResult<object>>
    {
        private readonly AutoPartsStoreDb _db;
        private readonly IMapper _mapper;

        public CancelSaleCommandHandler(AutoPartsStoreDb db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IDataResult<object>> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _db.Set<Sale>()
                .Include(s => s.Client)
                .Include(s => s.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(s => s.Id == request.SaleId && !s.IsDeleted, cancellationToken);

            if (sale == null)
                return new ErrorDataResult<object>("Sale not found", HttpStatusCode.NotFound);

            if (sale.Status != SaleStatus.Draft)
                return new ErrorDataResult<object>("Only draft sales can be cancelled", HttpStatusCode.BadRequest);

            sale.Status = SaleStatus.Cancelled;
            sale.ModifiedDate = DateTime.UtcNow.Ticks;
            _db.Set<Sale>().Update(sale);
            await _db.SaveChangesAsync(cancellationToken);

            var updated = await _db.Set<Sale>()
                .Include(s => s.Client)
                .Include(s => s.Items)
                .ThenInclude(i => i.Product)
                .FirstAsync(s => s.Id == sale.Id, cancellationToken);
            return new SuccessDataResult<object>(_mapper.Map<SaleDto>(updated), "Sale cancelled");
        }
    }
}
