using System.Net;
using AutoMapper;
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
        private readonly IMessagesRepository _messagesRepository;

        public CategoryEditCommandHandler(IValidator<CategoryFormDto> validator, ICategoryDal categoryDal,
            IMessagesRepository messagesRepository)
        {
            _validator = validator;
            _categoryDal = categoryDal;
            _messagesRepository = messagesRepository;
        }

        public async Task<IDataResult<object>> Handle(CategoryEditCommand request, CancellationToken cancellationToken)
        {
            var validationOfForm = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validationOfForm.IsValid)
                return new ErrorDataResult<object>(_messagesRepository.FormValidation(), HttpStatusCode.BadRequest,
                    validationOfForm.Errors.Select(e => e.ErrorMessage).ToList());

            var source = await _categoryDal.GetAsync(i => i.Id == request.Id);
            if (source == null)
                return new ErrorDataResult<object>(_messagesRepository.NotFound(), HttpStatusCode.NotFound);

            source.name = _xssRepository.Execute(request.Form.name);
            source.description = _xssRepository.Execute(request.Form.description);
            source.comments = _xssRepository.Execute(request.Form.comments);

            source.Name = request.Form.name;
            source.Description = request.Form.description;
            source.Comments = request.Form.comments;

            await _categoryDal.UpdateAsync(source);

            var categoryDto = _mapper.Map<CategoryDto>(source);

            return new SuccessDataResult<object>(categoryDto, _messagesRepository.Edited($"{source.Name}"));
        }
    }
}