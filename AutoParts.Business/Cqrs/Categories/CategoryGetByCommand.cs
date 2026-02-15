using System.Net;
using AutoMapper;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DtoModels.Category;
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

        private readonly ICategoryDal _categoryDal;
        private readonly IMapper _mapper;
        
        public CategoryGetByCommandHandler(ICategoryDal categoryDal, IMapper mapper)
        {
            _categoryDal = categoryDal;
            _mapper = mapper;
        }

        #endregion

        public async Task<IDataResult<CategoryDto>> Handle(CategoryGetByCommand request, CancellationToken cancellationToken)
        {
            var source = await _categoryDal.GetAsync(i => i.Id == request.Id);
            if (source == null)
                return new ErrorDataResult<CategoryDto>("Record not found", HttpStatusCode.NotFound);

            return new SuccessDataResult<CategoryDto>(_mapper.Map<CategoryDto>(source));
        }
    }
}