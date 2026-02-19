using AutoParts.DataAccess.Models.DtoModels.StockMovement;
using AutoParts.DataAccess.Models.Enums;
using FluentValidation;

namespace AutoParts.Business.Validators.StockMovement;

public class StockMovementFormValidator : AbstractValidator<StockMovementFormDto>
{
    public StockMovementFormValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Product is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.Type)
            .IsInEnum();

        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Reason));

        RuleFor(x => x.DocumentNo)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.DocumentNo));
    }
}
