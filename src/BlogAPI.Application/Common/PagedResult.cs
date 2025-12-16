namespace BlogAPI.Application.Common;

public class PagedResult<T>(IEnumerable<T> items, int totalCount, int page, int pageSize)
{
    public IEnumerable<T> Items { get; } = items;
    public int TotalCount { get; } = totalCount;
    public int Page { get; } = page;
    public int PageSize { get; } = pageSize;
    public int TotalPages { get; } = (int)Math.Ceiling(totalCount / (double)pageSize);
}
