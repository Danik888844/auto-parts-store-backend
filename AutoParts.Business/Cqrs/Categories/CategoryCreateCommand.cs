using System.Net;
using AutoMapper;
using AutoParts.Core.GeneralHelpers;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels.Category;
using FluentValidation;
using MediatR;

namespace AutoParts.Business.Cqrs.Categories;

public class CategoryCreateCommand : IRequest<IDataResult<object>>
{
    public CategoryFormDto Form { get; }

    public CategoryCreateCommand(CategoryFormDto form)
    {
        Form = form;
    }
    public class CategoryCreateCommandHandler : IRequestHandler<CategoryCreateCommand, IDataResult<object>>
    {
        #region DI

        private readonly ICategoryDal _categoryDal;
        private readonly IValidator<CategoryFormDto> _validator;
        private readonly IXssRepository _xssRepository;
        private readonly IMapper _mapper;

        public CategoryCreateCommandHandler(ICategoryDal categoryDal, IValidator<CategoryFormDto> validator, 
            IXssRepository xssRepository, IMapper mapper)
        {
            _categoryDal = categoryDal;
            _validator = validator;
            _xssRepository = xssRepository;
            _mapper = mapper;
        }

        #endregion

        public async Task<IDataResult<object>> Handle(CategoryCreateCommand request, CancellationToken cancellationToken)
        {            
            var validationOfForm = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validationOfForm.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validationOfForm.Errors.Select(e => e.ErrorMessage).ToList());

            request.Form.Name = _xssRepository.Execute(request.Form.Name);

            var source = _mapper.Map<Category>(request.Form);

            await _categoryDal.AddAsync(source);

            var categoryDto = _mapper.Map<CategoryDto>(source);

            return new SuccessDataResult<object>(categoryDto,$"Created: {source.Name}");
        } 
    }
}