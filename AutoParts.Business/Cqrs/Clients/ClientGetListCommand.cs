using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using AutoParts.Core.GeneralHelpers;
using AutoParts.Core.Results;
using AutoParts.DataAccess.Dals;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels;
using AutoParts.DataAccess.Models.DtoModels.Client;
using FluentValidation;
using MediatR;

namespace AutoParts.Business.Cqrs.Clients;

public class ClientGetListCommand : IRequest<IDataResult<object>>
{
    public PaginationFormDto Form { get; }

    public ClientGetListCommand(PaginationFormDto form)
    {
        Form = form;
    }

    public class ClientGetListCommandHandler : IRequestHandler<ClientGetListCommand, IDataResult<object>>
    {
        #region DI

        private readonly IClientDal _clientDal;
        private readonly IValidator<PaginationFormDto> _validator;
        private readonly IMapper _mapper;

        public ClientGetListCommandHandler(IClientDal clientDal,
            IValidator<PaginationFormDto> validator, IMapper mapper)
        {
            _clientDal = clientDal;
            _validator = validator;
            _mapper = mapper;
        }

        #endregion

        public async Task<IDataResult<object>> Handle(ClientGetListCommand request, CancellationToken cancellationToken)
        {
            var validationOfForm = await _validator.ValidateAsync(request.Form, cancellationToken);
            if (!validationOfForm.IsValid)
                return new ErrorDataResult<object>("Form validation error", HttpStatusCode.BadRequest,
                    validationOfForm.Errors.Select(e => e.ErrorMessage).ToList());

            Expression<Func<Client, bool>>? filter = null;
            if (!string.IsNullOrWhiteSpace(request.Form.Search))
            {
                var s = request.Form.Search.Trim().ToLower();
                filter = x => x.FullName.ToLower().Contains(s)
                    || (x.Phone != null && x.Phone.ToLower().Contains(s))
                    || (x.Email != null && x.Email.ToLower().Contains(s))
                    || (x.Notes != null && x.Notes.ToLower().Contains(s));
            }

            var source = await _clientDal.GetAllPagedAsync(filter, request.Form.PageNumber, request.Form.ViewSize);

            var clients = source.Items.Select(i => _mapper.Map<ClientDto>(i)).ToList();

            return new SuccessDataResult<object>(new PaginationReturnListDto<ClientDto>
            {
                Items = clients,
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
