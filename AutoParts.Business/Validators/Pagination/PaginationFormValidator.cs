using AutoParts.DataAccess.Models.DtoModels;
using FluentValidation;

namespace AutoParts.Business.Validators.Pagination;

public class PaginationFormValidator : AbstractValidator<PaginationFormDto>
{
    public PaginationFormValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);
        
        RuleFor(x => x.ViewSize)
            .GreaterThan(0)
            .LessThan(100);
    }
}