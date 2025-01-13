using System;
using Microsoft.EntityFrameworkCore;

namespace API.RequestHelpers;

public class PagedList<T> 
{
    public PagedList(IReadOnlyList<T> items, int count, int pageNumber, int pageSize)
    {
        CurrentPage = pageNumber;

        TotalPages = (int)Math.Ceiling(count / (double)pageSize);

        PageSize = pageSize;

        TotalCount = count;

        Items = items;
    }

    public int CurrentPage { get; set; }

    public int TotalPages { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }
    public IReadOnlyList<T> Items { get; set; }

    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = await source.CountAsync();

        var totalPages = (int)Math.Ceiling(count / (double)pageSize);

        //Adjust pageNumber if it is greater than the totalPages
        if (pageNumber > totalPages && totalPages > 0)
        {
            pageNumber = totalPages;
        }
        else if (totalPages == 0)
        {
            pageNumber = 1;
        }

        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}
