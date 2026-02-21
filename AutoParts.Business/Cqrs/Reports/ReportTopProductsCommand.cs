using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DtoModels.Report;
using AutoParts.DataAccess.Models.Enums;
using MediatR;

namespace AutoParts.Business.Cqrs.Reports;

public class ReportTopProductsCommand : IRequest<IDataResult<ReportTopProductsDto>>
{
    public ReportTopProductsFilterDto Filter { get; }

    public ReportTopProductsCommand(ReportTopProductsFilterDto filter)
    {
        Filter = filter;
    }

    public class ReportTopProductsCommandHandler : IRequestHandler<ReportTopProductsCommand, IDataResult<ReportTopProductsDto>>
    {
        private readonly IReportDal _reportDal;

        public ReportTopProductsCommandHandler(IReportDal reportDal)
        {
            _reportDal = reportDal;
        }

        public async Task<IDataResult<ReportTopProductsDto>> Handle(ReportTopProductsCommand request, CancellationToken cancellationToken)
        {
            var filter = request.Filter;
            filter.DateFrom = DateTime.SpecifyKind(filter.DateFrom.Date, DateTimeKind.Utc);
            filter.DateTo = DateTime.SpecifyKind(filter.DateTo.Date.AddDays(1), DateTimeKind.Utc);

            var dto = await _reportDal.GetTopProductsAsync(filter, cancellationToken);

            dto.ChartData = dto.Table
                .Select(r => new ReportTopProductsChartPointDto
                {
                    X = r.Sku + " " + r.Name,
                    Y = filter.OrderBy == TopProductsOrderBy.ByQuantity ? r.Qty : r.Revenue
                })
                .ToList();

            return new SuccessDataResult<ReportTopProductsDto>(dto);
        }
    }
}
