using AutoParts.Core.GeneralHelpers;

namespace AutoParts.DataAccess.Models.DtoModels;

public class PaginationReturnListDto<T> where T : DtoBaseEntity
{
    public List<T> Items { get; set; } = [];
    public PaginationReturnModel Pagination { get; set; } = new();
}