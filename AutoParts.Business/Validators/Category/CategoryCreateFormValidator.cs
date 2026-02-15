using AutoParts.Business.Cqrs.Categories;
using AutoParts.DataAccess.Models.DtoModels.Category;
using FluentValidation;

namespace AutoParts.Business.Validators.Category;

public class CategoryCreateFormValidator : AbstractValidator<CategoryFormDto>
{
    public CategoryCreateFormValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("No more 100 characters long");
    }
}