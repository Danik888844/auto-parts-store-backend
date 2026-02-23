using AutoParts.Business.Services.Identity;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DtoModels.Report;
using AutoParts.DataAccess.Models.Enums;
using ClosedXML.Excel;
using MediatR;

namespace AutoParts.Business.Cqrs.Reports;

public class ReportSalesByPeriodExportCommand : IRequest<IDataResult<byte[]>>
{
    public ReportSalesByPeriodFilterDto Filter { get; }

    public ReportSalesByPeriodExportCommand(ReportSalesByPeriodFilterDto filter)
    {
        Filter = filter;
    }

    public class ReportSalesByPeriodExportCommandHandler : IRequestHandler<ReportSalesByPeriodExportCommand, IDataResult<byte[]>>
    {
        private readonly IReportDal _reportDal;
        private readonly IIdentityUserApiClient _identityUserApi;

        public ReportSalesByPeriodExportCommandHandler(IReportDal reportDal, IIdentityUserApiClient identityUserApi)
        {
            _reportDal = reportDal;
            _identityUserApi = identityUserApi;
        }

        public async Task<IDataResult<byte[]>> Handle(ReportSalesByPeriodExportCommand request, CancellationToken cancellationToken)
        {
            var filter = request.Filter;
            filter.DateFrom = DateTime.SpecifyKind(filter.DateFrom.Date, DateTimeKind.Utc);
            filter.DateTo = DateTime.SpecifyKind(filter.DateTo.Date.AddDays(1), DateTimeKind.Utc);

            var rows = await _reportDal.GetSalesForPeriodExportAsync(filter, cancellationToken);

            var userIds = rows.Select(r => r.SellerName).Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
            if (userIds.Count > 0)
            {
                var displayNames = await _identityUserApi.GetDisplayNamesAsync(userIds, cancellationToken);
                foreach (var r in rows)
                {
                    if (!string.IsNullOrEmpty(r.SellerName) && displayNames.TryGetValue(r.SellerName, out var name))
                        r.SellerName = name;
                }
            }

            using var workbook = new XLWorkbook();
            var sheet = workbook.AddWorksheet("Продажи за период");

            sheet.Cell(1, 1).Value = "№";
            sheet.Cell(1, 2).Value = "Дата";
            sheet.Cell(1, 3).Value = "Продавец";
            sheet.Cell(1, 4).Value = "Способ оплаты";
            sheet.Cell(1, 5).Value = "Статус";
            sheet.Cell(1, 6).Value = "Сумма";
            sheet.Cell(1, 7).Value = "Кол-во единиц";
            sheet.Range(1, 1, 1, 7).Style.Font.Bold = true;

            var row = 2;
            foreach (var r in rows)
            {
                sheet.Cell(row, 1).Value = r.Id;
                sheet.Cell(row, 2).Value = r.SoldAt;
                sheet.Cell(row, 3).Value = r.SellerName ?? "";
                sheet.Cell(row, 4).Value = PaymentTypeName(r.PaymentType);
                sheet.Cell(row, 5).Value = StatusName(r.Status);
                sheet.Cell(row, 6).Value = r.Total;
                sheet.Cell(row, 7).Value = r.ItemsQuantity;
                row++;
            }

            sheet.Columns().AdjustToContents();

            await using var ms = new MemoryStream();
            workbook.SaveAs(ms, new SaveOptions { ValidatePackage = false });
            var bytes = ms.ToArray();
            return new SuccessDataResult<byte[]>(bytes);
        }

        private static string PaymentTypeName(PaymentType t) => t switch
        {
            PaymentType.Cash => "Наличные",
            PaymentType.Card => "Карта",
            PaymentType.Transfer => "Перевод",
            _ => t.ToString()
        };

        private static string StatusName(SaleStatus s) => s switch
        {
            SaleStatus.Completed => "Завершена",
            SaleStatus.Refunded => "Возврат",
            SaleStatus.Cancelled => "Отменена",
            SaleStatus.Draft => "Черновик",
            _ => s.ToString()
        };
    }
}
