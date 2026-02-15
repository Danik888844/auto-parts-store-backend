using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace AutoParts.Business.Cqrs.Categories;

public class CategoryGetByCommand : IRequest<IDataResult<CategoryDto>>
{
    public int Id { get; }

    public CategoryGetByCommand(int id)
    {
        Id = id;
    }

    public class CategoryGetByCommandHandler : IRequestHandler<CategoryGetByCommand, IDataResult<CategoryDto>>
    {
        #region DI

        private readonly IValidator<int> _validator;
        private readonly IMessagesRepository _messagesRepository;
        private readonly ICategoryDal _categoryDal;
        private readonly IMapper _mapper;
        
        public CategoryGetByCommandHandler(IValidator<int> validator, IMessagesRepository messagesRepository,
            ICategoryDal categoryDal, IMapper mapper)
        {
            _validator = validator;
            _messagesRepository = messagesRepository;
            _categoryDal = categoryDal;
            _mapper = mapper;
        }

        #endregion

        public async Task<IDataResult<CategoryDto>> Handle(CategoryGetByCommand request, CancellationToken cancellationToken)
        {
            var source = await _categoryDal.GetAsync(i => i.Id == request.Id);
            if (source == null)
                return new ErrorDataResult<CategoryDto>(_messagesRepository.NotFound(), HttpStatusCode.NotFound);

            return new SuccessDataResult<CategoryDto>(_mapper.Map<CategoryDto>(source));
        }
    }
}