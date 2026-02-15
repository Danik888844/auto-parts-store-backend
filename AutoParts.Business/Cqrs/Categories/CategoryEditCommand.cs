using System.Net;
using AutoMapper;
using AutoParts.Core.GeneralHelpers;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DtoModels.Category;
using FluentValidation;
using MediatR;
using Newtonsoft.Json;

namespace AutoParts.Business.Cqrs.Categories;

public class CategoryEditCommand : IRequest<IDataResult<object>>
{
    public int Id { get; }
    public CategoryFormDto Form { get; }

    public CategoryEditCommand(int id, CategoryFormDto form)
    {
        Id = id;
        Form = form;
    }

    public class CategoryEditCommandHandler : IRequestHandler<CategoryEditCommand, IDataResult<object>>
    {
        private readonly IValidator<CategoryFormDto> _validator;
        private readonly ICategoryDal _categoryDal;
        private readonly IXssRepository _xssRepository;
        private readonly IMapper _mapper;

        public CategoryEditCommandHandler(IValidator<CategoryFormDto> validator, ICategoryDal categoryDal, 
            IXssRepository xssRepository, IMapper mapper)
        {
            _validator = validator;
            _categoryDal = categoryDal;
            _xssRepository = xssRepository;
            _mapper = mapper;
        }

        public async Task<IDataResult<object>> Handle(CategoryEditCommand request, CancellationToken cancellationToken)
        {
            var validationOfForm = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validationOfForm.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validationOfForm.Errors.Select(e => e.ErrorMessage).ToList());

            var source = await _categoryDal.GetAsync(i => i.Id == request.Id);
            if (source == null)
                return new ErrorDataResult<object>("Record not found", HttpStatusCode.NotFound);

            source.Name = _xssRepository.Execute(request.Form.Name);

            source.Name = request.Form.Name;

            await _categoryDal.UpdateAsync(source);

            var categoryDto = _mapper.Map<CategoryDto>(source);

            return new SuccessDataResult<object>(categoryDto, $"\"{source.Name}\" edited");
        }
    }
}