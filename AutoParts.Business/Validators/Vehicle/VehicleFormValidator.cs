using AutoParts.DataAccess.Models.DtoModels.Vehicle;
using FluentValidation;

namespace AutoParts.Business.Validators.Vehicle;

public class VehicleFormValidator : AbstractValidator<VehicleFormDto>
{
    public VehicleFormValidator()
    {
        RuleFor(x => x.ModelId)
            .GreaterThan(0)
            .WithMessage("Model is required");

        RuleFor(x => x.YearFrom)
            .InclusiveBetween(1900, 2100)
            .WithMessage("Year must be between 1900 and 2100");

        RuleFor(x => x.YearTo)
            .InclusiveBetween(1900, 2100)
            .WithMessage("Year must be between 1900 and 2100");

        RuleFor(x => x.YearTo)
            .GreaterThanOrEqualTo(x => x.YearFrom)
            .WithMessage("YearTo must be >= YearFrom");

        RuleFor(x => x.Generation)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Generation));

        RuleFor(x => x.Engine)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Engine));

        RuleFor(x => x.BodyType)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.BodyType));

        RuleFor(x => x.Note)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Note));
    }
}
