using AutoParts.DataAccess.Models.DtoModels.VehicleBrand;
using FluentValidation;

namespace AutoParts.Business.Validators.VehicleBrand;

public class VehicleBrandFormValidator : AbstractValidator<VehicleBrandFormDto>
{
    public VehicleBrandFormValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("No more than 100 characters");
    }
}
