using AutoParts.DataAccess.Models.DtoModels.Sale;
using FluentValidation;

namespace AutoParts.Business.Validators.Sale;

public class SaleFormValidator : AbstractValidator<SaleFormDto>
{
    public SaleFormValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Sale must have at least one item");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("Product is required");
            item.RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0");
        });
    }
}
