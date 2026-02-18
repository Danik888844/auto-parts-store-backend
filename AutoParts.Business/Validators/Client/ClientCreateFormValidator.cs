using AutoParts.DataAccess.Models.DtoModels.Client;
using FluentValidation;

namespace AutoParts.Business.Validators.Client;

public class ClientCreateFormValidator : AbstractValidator<ClientFormDto>
{
    public ClientCreateFormValidator()
    {
        RuleFor(x => x.FullName)
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

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Notes))
            .WithMessage("No more 1000 characters long");
    }
}
