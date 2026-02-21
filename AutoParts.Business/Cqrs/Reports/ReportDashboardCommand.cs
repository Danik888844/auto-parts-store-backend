using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DtoModels.Report;
using MediatR;

namespace AutoParts.Business.Cqrs.Reports;

public class ReportDashboardCommand : IRequest<IDataResult<ReportDashboardDto>>
{
    public class ReportDashboardCommandHandler : IRequestHandler<ReportDashboardCommand, IDataResult<ReportDashboardDto>>
    {
        private readonly IReportDal _reportDal;

        public ReportDashboardCommandHandler(IReportDal reportDal)
        {
            _reportDal = reportDal;
        }

        public async Task<IDataResult<ReportDashboardDto>> Handle(ReportDashboardCommand request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var todayStart = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
            // Текущая календарная неделя (пн–вс): начало — понедельник 00:00
            var daysSinceMonday = ((int)now.DayOfWeek - 1 + 7) % 7;
            var weekStart = todayStart.AddDays(-daysSinceMonday);

            var data = await _reportDal.GetDashboardAsync(todayStart, weekStart);

            var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var monthEnd = monthStart.AddMonths(1);
            var salesByMonth = await _reportDal.GetSalesByMonthAsync(monthStart, monthEnd);
            data.SalesByMonth = salesByMonth.Data;

            return new SuccessDataResult<ReportDashboardDto>(data);
        }
    }
}
