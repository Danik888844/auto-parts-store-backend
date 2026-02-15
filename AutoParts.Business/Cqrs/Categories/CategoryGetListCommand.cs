using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
        private readonly IGrpcIdentityUserRepository _grpcIdentityUserRepository;
        private readonly IPaginationRepository _paginationRepository;
        private readonly IValidator<PaginationFormDto> _validator;
        private readonly IMessagesRepository _messagesRepository;
        private readonly IMapper _mapper;

        public CategoryGetListCommandHandler(IPaginationRepository paginationRepository, ICategoryDal categoryDal,
            IValidator<PaginationFormDto> validator, IMessagesRepository messagesRepository, IMapper mapper)
        {
            _paginationRepository = paginationRepository;
            _categoryDal = categoryDal;
            _validator = validator;
            _messagesRepository = messagesRepository;
            _mapper = mapper;
            _xssRepository = xssRepository;
            _mapper = mapper;
        }

        #endregion

        public async Task<IDataResult<object>> Handle(CategoryGetListCommand request, CancellationToken cancellationToken)
        {           
            var validationOfForm = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validationOfForm.IsValid)
                return new ErrorDataResult<object>(_messagesRepository.FormValidation(), HttpStatusCode.BadRequest,
                    validationOfForm.Errors.Select(e => e.ErrorMessage).ToList());

            var source = await _categoryDal.GetAllAsync();

            var categories = source.Select(i => _mapper.Map<CategoryDto>(i)).ToList();

            return new SuccessDataResult<object>(new CategoryListDto()
            {
                categories = categories,
                pagination = pagination,
            });
        }
        
    }
}