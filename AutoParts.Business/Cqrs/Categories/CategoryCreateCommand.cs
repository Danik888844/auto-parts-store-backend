using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using Newtonsoft.Json;

namespace AutoParts.Business.Cqrs.Categories;

public class CategoryCreateCommand : IRequest<IDataResult<object>>
{
    public CategoryCreateCommandDto Form { get; }

    public CategoryCreateCommand(CategoryCreateCommandDto form)
    {
        Form = form;
    }
    public class CategoryCreateCommandHandler : IRequestHandler<CategoryCreateCommand, IDataResult<object>>
    {
        #region DI

        private readonly IMessagesRepository _messagesRepository;
        private readonly ICategoryDal _categoryDal;
       

        public CategoryCreateCommandHandler(IMessagesRepository messagesRepository, ICategoryDal categoryDal,
            IValidator<CategoryCreateCommandDto> validator)
        {
            _messagesRepository = messagesRepository;
            _categoryDal = categoryDal;
            _validator = validator;
        }

        #endregion

        public async Task<IDataResult<object>> Handle(CategoryCreateCommand request, CancellationToken cancellationToken)
        {            
            var validationOfForm = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validationOfForm.IsValid)
                return new ErrorDataResult<object>(_messagesRepository.FormValidation(), HttpStatusCode.BadRequest,
                    validationOfForm.Errors.Select(e => e.ErrorMessage).ToList());

            request.Form.name = _xssRepository.Execute(request.Form.name);
            request.Form.description = _xssRepository.Execute(request.Form.description);
            request.Form.comments = _xssRepository.Execute(request.Form.comments);

            var source = _mapper.Map<Category>(request.Form);

            await _categoryDal.AddAsync(source);

            var categoryDto = _mapper.Map<CategoryDto>(source);

            return new SuccessDataResult<object>(categoryDto,
                _messagesRepository.Created($"{source.Name}"));
        } 
    }
}