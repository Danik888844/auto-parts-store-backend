using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DtoModels.Report;
using MediatR;

namespace AutoParts.Business.Cqrs.Reports;

public class ReportSalesByPeriodCommand : IRequest<IDataResult<ReportSalesByPeriodDto>>
{
    public ReportSalesByPeriodFilterDto Filter { get; }

    public ReportSalesByPeriodCommand(ReportSalesByPeriodFilterDto filter)
    {
        Filter = filter;
    }

    public class ReportSalesByPeriodCommandHandler : IRequestHandler<ReportSalesByPeriodCommand, IDataResult<ReportSalesByPeriodDto>>
    {
        private readonly IReportDal _reportDal;

        public ReportSalesByPeriodCommandHandler(IReportDal reportDal)
        {
            _reportDal = reportDal;
        }

        public async Task<IDataResult<ReportSalesByPeriodDto>> Handle(ReportSalesByPeriodCommand request, CancellationToken cancellationToken)
        {
            var filter = request.Filter;
            filter.DateFrom = DateTime.SpecifyKind(filter.DateFrom.Date, DateTimeKind.Utc);
            filter.DateTo = DateTime.SpecifyKind(filter.DateTo.Date.AddDays(1), DateTimeKind.Utc);

            var data = await _reportDal.GetSalesByPeriodAsync(filter, cancellationToken);
            return new SuccessDataResult<ReportSalesByPeriodDto>(data);
        }
    }
}
