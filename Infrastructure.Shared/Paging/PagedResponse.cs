namespace Infrastructure.Shared.Paging;

public class PagedResponse<TResponse>
{
  public PagingMetaData PagingMetaData { get; set; }
  public List<TResponse> Data { get; set; }
}
