using AutoParts.DataAccess.Models.DtoModels.Manufacturer;
using FluentValidation;

namespace AutoParts.Business.Validators.Manufacturer;

public class ManufacturerCreateFormValidator : AbstractValidator<ManufacturerFormDto>
{
    public ManufacturerCreateFormValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("No more 100 characters long");

        RuleFor(x => x.Country)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Country))
            .WithMessage("No more 100 characters long");
    }
}
