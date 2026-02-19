using AutoParts.DataAccess.Models.DtoModels.Stock;
using FluentValidation;

namespace AutoParts.Business.Validators.Stock;

public class StockListFormValidator : AbstractValidator<StockListFormDto>
{
    public StockListFormValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.ViewSize).GreaterThan(0).LessThanOrEqualTo(100);
    }
}
