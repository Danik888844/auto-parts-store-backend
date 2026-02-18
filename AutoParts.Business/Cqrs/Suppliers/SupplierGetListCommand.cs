using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using AutoParts.Core.GeneralHelpers;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels;
using AutoParts.DataAccess.Models.DtoModels.Supplier;
using FluentValidation;
using MediatR;

namespace AutoParts.Business.Cqrs.Suppliers;

public class SupplierGetListCommand : IRequest<IDataResult<object>>
{
    public PaginationFormDto Form { get; }

    public SupplierGetListCommand(PaginationFormDto form)
    {
        Form = form;
    }

    public class SupplierGetListCommandHandler : IRequestHandler<SupplierGetListCommand, IDataResult<object>>
    {
        #region DI

        private readonly ISupplierDal _supplierDal;
        private readonly IValidator<PaginationFormDto> _validator;
        private readonly IMapper _mapper;

        public SupplierGetListCommandHandler(ISupplierDal supplierDal,
            IValidator<PaginationFormDto> validator, IMapper mapper)
        {
            _supplierDal = supplierDal;
            _validator = validator;
            _mapper = mapper;
        }

        #endregion

        public async Task<IDataResult<object>> Handle(SupplierGetListCommand request, CancellationToken cancellationToken)
        {
            var validationOfForm = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validationOfForm.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validationOfForm.Errors.Select(e => e.ErrorMessage).ToList());

            Expression<Func<Supplier, bool>> filter = x => !x.IsDeleted;
            if (!string.IsNullOrWhiteSpace(request.Form.Search))
            {
                var s = request.Form.Search.Trim().ToLower();
                filter = x => !x.IsDeleted && (x.Name.ToLower().Contains(s)
                    || (x.Phone != null && x.Phone.ToLower().Contains(s))
                    || (x.Email != null && x.Email.ToLower().Contains(s))
                    || (x.Address != null && x.Address.ToLower().Contains(s)));
            }

            var source = await _supplierDal.GetAllPagedAsync(filter, request.Form.PageNumber, request.Form.ViewSize);

            var suppliers = source.Items.Select(i => _mapper.Map<SupplierDto>(i)).ToList();

            return new SuccessDataResult<object>(new PaginationReturnListDto<SupplierDto>
            {
                Items = suppliers,
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
