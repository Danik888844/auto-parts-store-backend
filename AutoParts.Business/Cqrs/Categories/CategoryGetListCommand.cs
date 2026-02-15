using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using AutoParts.Core.GeneralHelpers;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels;
using AutoParts.DataAccess.Models.DtoModels.Category;
using FluentValidation;
using MediatR;

namespace AutoParts.Business.Cqrs.Categories;

public class CategoryGetListCommand : IRequest<IDataResult<object>>
{
    public PaginationFormDto Form { get; }

    public CategoryGetListCommand(PaginationFormDto form)
    {
        Form = form;
    }

    public class CategoryGetListCommandHandler : IRequestHandler<CategoryGetListCommand, IDataResult<object>>
    {
        #region DI

        private readonly ICategoryDal _categoryDal;
        private readonly IValidator<PaginationFormDto> _validator;
        private readonly IMapper _mapper;

        public CategoryGetListCommandHandler(ICategoryDal categoryDal,
            IValidator<PaginationFormDto> validator, IMapper mapper)
        {
            _categoryDal = categoryDal;
            _validator = validator;
            _mapper = mapper;
        }

        #endregion

        public async Task<IDataResult<object>> Handle(CategoryGetListCommand request, CancellationToken cancellationToken)
        {           
            var validationOfForm = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validationOfForm.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validationOfForm.Errors.Select(e => e.ErrorMessage).ToList());

            Expression<Func<Category, bool>>? filter = null;
            if (!string.IsNullOrWhiteSpace(request.Form.Search))
            {
                var s = request.Form.Search.Trim();
                filter = c => c.Name.Contains(s);
            }

            var source = await _categoryDal.GetAllPagedAsync(filter, request.Form.PageNumber, request.Form.ViewSize);

            var categories = source.Items.Select(i => _mapper.Map<CategoryDto>(i)).ToList();

            return new SuccessDataResult<object>(new PaginationReturnListDto<CategoryDto>
            {
                Items = categories,
                Pagination = new PaginationReturnModel()
                {
                    CurrentPage =  request.Form.PageNumber,
                    PageSize = request.Form.ViewSize,
                    TotalItems = source.TotalCount,
                    TotalPages = source.TotalPages
                },
            });
        }
        
    }
}