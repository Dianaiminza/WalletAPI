using Infrastructure.Shared.CustomExceptions;
using Infrastructure.Shared.Paging;
using Microsoft.EntityFrameworkCore;

namespace CoreApi.Infrastructure.Extensions;

public static class QueryableExtensions
{
  public static async Task<PagedList<T>> ToPaginatedListAsync<T>(this IQueryable<T> source, int pageNumber,
      int pageSize) where T : class
  {
    if (source == null) throw new ApiException("List for pagination cannot be null");
    pageNumber = pageNumber == 0 ? 1 : pageNumber;
    pageSize = pageSize == 0 ? 10 : pageSize;
    var count = await source.CountAsync();
    pageNumber = pageNumber <= 0 ? 1 : pageNumber;
    var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    return PagedList<T>.ToPagedList(items, count, pageNumber, pageSize);
  }

  public static IQueryable<T> GetPage<T>(this IQueryable<T> list, int pageNumber, int pageSize)
  {
    return list
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize);
  }

  public static IQueryable<T> TrackChanges<T>(this IQueryable<T> list, bool trackChanges = false) where T : class
  {
    if (trackChanges) return list;

    return list
        .AsNoTracking();
  }
}
