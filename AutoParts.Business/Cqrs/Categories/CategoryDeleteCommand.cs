using System.Net;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
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
        private readonly ICategoryDal _categoryDal;

        public CategoryDeleteCommandHandler(ICategoryDal categoryDal)
        {
            _categoryDal = categoryDal;
        }

        public async Task<IDataResult<object>> Handle(CategoryDeleteCommand request, CancellationToken cancellationToken)
        {   
            var source = await _categoryDal.GetAsync(i => i.Id == request.Id && !i.IsDeleted);
            if (source == null)
                return new ErrorDataResult<object>("Record not found", HttpStatusCode.NotFound);

            source.IsDeleted = true;
            await _categoryDal.UpdateAsync(source);

            return new SuccessDataResult<object>("Deleted");
        }
    }
}