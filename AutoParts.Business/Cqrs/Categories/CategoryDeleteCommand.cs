using System.Net;
using FluentValidation;
using AutoMapper;
using Newtonsoft.Json;
using Hangfire;
using MediatR;

namespace AutoParts.Business.Cqrs.Categories;

public class CategoryDeleteCommand : IRequest<IDataResult<object>>
{
    public int Id { get; }

    public CategoryDeleteCommand(int id)
    {
        Id = id;
    }

    public class CategoryDeleteCommandHandler : IRequestHandler<CategoryDeleteCommand,
        IDataResult<object>>
    {
        private readonly IValidator<int> _validator;
        private readonly ICategoryDal _categoryDal;
        private readonly IMessagesRepository _messagesRepository;

        public CategoryDeleteCommandHandler(IValidator<int> validator, ICategoryDal categoryDal,
            IMessagesRepository messagesRepository)
        {
            _validator = validator;
            _categoryDal = categoryDal;
            _messagesRepository = messagesRepository;
        }

        public async Task<IDataResult<object>> Handle(CategoryDeleteCommand request, CancellationToken cancellationToken)
        {   
            var validationOfForm = await _validator.ValidateAsync(request.Id);
            if (!validationOfForm.IsValid)
                return new ErrorDataResult<object>(_messagesRepository.FormValidation(), HttpStatusCode.BadRequest,
                    validationOfForm.Errors.Select(e => e.ErrorMessage).ToList());

            var source = await _categoryDal.GetAsync(i => i.Id == request.Id);
            if (source == null)
                return new ErrorDataResult<object>(_messagesRepository.NotFound(), HttpStatusCode.NotFound);

            await _categoryDal.DeleteAsync(source);

            return new SuccessDataResult<object>(_messagesRepository.Deleted());
        }
    }
}