using AutoParts.DataAccess.Models.DtoModels.VehicleModel;
using FluentValidation;

namespace AutoParts.Business.Validators.VehicleModel;

public class VehicleModelFormValidator : AbstractValidator<VehicleModelFormDto>
{
    public VehicleModelFormValidator()
    {
        RuleFor(x => x.BrandId)
            .GreaterThan(0)
            .WithMessage("Brand is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("No more than 100 characters");
    }
}
