using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using AutoParts.Core.GeneralHelpers;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels;
using AutoParts.DataAccess.Models.DtoModels.Manufacturer;
using FluentValidation;
using MediatR;

namespace AutoParts.Business.Cqrs.Manufacturers;

public class ManufacturerGetListCommand : IRequest<IDataResult<object>>
{
    public PaginationFormDto Form { get; }

    public ManufacturerGetListCommand(PaginationFormDto form)
    {
        Form = form;
    }

    public class ManufacturerGetListCommandHandler : IRequestHandler<ManufacturerGetListCommand, IDataResult<object>>
    {
        #region DI

        private readonly IManufacturerDal _manufacturerDal;
        private readonly IValidator<PaginationFormDto> _validator;
        private readonly IMapper _mapper;

        public ManufacturerGetListCommandHandler(IManufacturerDal manufacturerDal,
            IValidator<PaginationFormDto> validator, IMapper mapper)
        {
            _manufacturerDal = manufacturerDal;
            _validator = validator;
            _mapper = mapper;
        }

        #endregion

        public async Task<IDataResult<object>> Handle(ManufacturerGetListCommand request, CancellationToken cancellationToken)
        {
            var validationOfForm = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validationOfForm.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validationOfForm.Errors.Select(e => e.ErrorMessage).ToList());

            Expression<Func<Manufacturer, bool>>? filter = null;
            if (!string.IsNullOrWhiteSpace(request.Form.Search))
            {
                var s = request.Form.Search.Trim().ToLower();
                filter = m => m.Name.ToLower().Contains(s) || (m.Country != null && m.Country.ToLower().Contains(s));
            }

            var source = await _manufacturerDal.GetAllPagedAsync(filter, request.Form.PageNumber, request.Form.ViewSize);

            var manufacturers = source.Items.Select(i => _mapper.Map<ManufacturerDto>(i)).ToList();

            return new SuccessDataResult<object>(new PaginationReturnListDto<ManufacturerDto>
            {
                Items = manufacturers,
                Pagination = new PaginationReturnModel()
                {
                    CurrentPage = request.Form.PageNumber,
                    PageSize = request.Form.ViewSize,
                    TotalItems = source.TotalCount,
                    TotalPages = source.TotalPages
                },
            });
        }
    }
}
