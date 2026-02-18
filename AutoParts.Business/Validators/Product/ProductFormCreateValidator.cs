using AutoParts.DataAccess.Models.DtoModels.Product;
using FluentValidation;

namespace AutoParts.Business.Validators.Product;

public class ProductFormCreateValidator : AbstractValidator<ProductFormCreateDto>
{
    public ProductFormCreateValidator()
    {
        RuleFor(x => x.Sku)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("No more than 50 characters");

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("No more than 200 characters");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("Category is required");

        RuleFor(x => x.ManufacturerId)
            .GreaterThan(0)
            .WithMessage("Manufacturer is required");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price must be non-negative");

        RuleFor(x => x.PurchasePrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.PurchasePrice.HasValue)
            .WithMessage("Purchase price must be non-negative");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage("No more than 2000 characters");
    }
}
