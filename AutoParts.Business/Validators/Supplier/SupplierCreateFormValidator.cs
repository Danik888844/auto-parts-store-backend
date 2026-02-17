using AutoParts.DataAccess.Models.DtoModels.Supplier;
using FluentValidation;

namespace AutoParts.Business.Validators.Supplier;

public class SupplierCreateFormValidator : AbstractValidator<SupplierFormDto>
{
    public SupplierCreateFormValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("No more 200 characters long");

        RuleFor(x => x.Phone)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.Phone))
            .WithMessage("No more 50 characters long");

        RuleFor(x => x.Email)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("No more 100 characters long");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Invalid email format");

        RuleFor(x => x.Address)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Address))
            .WithMessage("No more 500 characters long");
    }
}
