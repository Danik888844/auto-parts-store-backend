using AutoParts.DataAccess.Models.DtoModels.ProductCompatibility;
using FluentValidation;

namespace AutoParts.Business.Validators.ProductCompatibility;

public class ProductCompatibilityFormValidator : AbstractValidator<ProductCompatibilityFormDto>
{
    public ProductCompatibilityFormValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Product is required");

        RuleFor(x => x.VehicleId)
            .GreaterThan(0)
            .WithMessage("Vehicle is required");

        RuleFor(x => x.Comment)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Comment));
    }
}
