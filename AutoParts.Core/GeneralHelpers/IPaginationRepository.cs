namespace AutoParts.Core.GeneralHelpers;

public class PaginationReturnModel
{
    
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int StartPage { get; set; }
    public int EndPage { get; set; }
}

public interface IPaginationRepository
{
    PaginationReturnModel GetPagination(int totalItems, int currentPage, int pageSize);
}

public class PaginationRepository : IPaginationRepository
{
    public PaginationReturnModel GetPagination(int totalItems, int currentPage, int pageSize)
    {
        pageSize = pageSize > 0 ? pageSize : 10;

        var totalPages = (int)Math.Ceiling((decimal)totalItems / pageSize);
        var startPage = currentPage - 5;
        var endPage = currentPage + 4;
        if (startPage <= 0)
        {
            endPage -= (startPage - 1);
            startPage = 1;
        }

        if (endPage > totalPages)
        {
            endPage = totalPages;
            if (endPage > 10)
            {
                startPage = endPage - 9;
            }
        }

        return new PaginationReturnModel
        {
            TotalItems = totalItems,
            CurrentPage = currentPage,
            PageSize = pageSize,
            TotalPages = totalPages,
            StartPage = startPage,
            EndPage = endPage
        };
    }
}