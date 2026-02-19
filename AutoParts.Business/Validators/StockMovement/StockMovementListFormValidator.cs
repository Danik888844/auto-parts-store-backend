using AutoParts.DataAccess.Models.DtoModels.StockMovement;
using FluentValidation;

namespace AutoParts.Business.Validators.StockMovement;

public class StockMovementListFormValidator : AbstractValidator<StockMovementListFormDto>
{
    public StockMovementListFormValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.ViewSize).GreaterThan(0).LessThanOrEqualTo(100);
    }
}
