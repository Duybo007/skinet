using System;

namespace API.RequestHelpers;

public class Pagination<T>(int pageIndex, int pageSize, int count, IReadOnlyList<T> data)
{
    public int PageIndex { get; set; } = pageIndex; // page number
    public int PageSize { get; set; } = pageSize;
    public int Count { get; set; } = count; // total items
    public IReadOnlyList<T> Data { get; set; } = data;
}
