namespace Shared.Common.Models;

public class PagedResult<T> where T : class
{
    public IReadOnlyList<T> Items { get; set; } = [];
    public int TotalCount { get; set; } 
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}