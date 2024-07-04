namespace Infrastructure.Shared.Paging;

public class PagedList<T> : List<T>
{
  public PagedList(List<T> items, int count, int pageNumber, int pageSize)
  {
    MetaData = new PagingMetaData
    {
      TotalCount = count,
      PageSize = pageSize,
      CurrentPage = pageNumber,
      TotalPages = (int)Math.Ceiling(count / (double)pageSize)
    };
    AddRange(items);
  }

  public PagingMetaData MetaData { get; set; }

  public static PagedList<T> ToPagedList(IList<T> source, int count, int pageNumber, int pageSize)
  {
    var items = source.ToList();
    return new PagedList<T>(items, count, pageNumber, pageSize);
  }
}
