namespace Application.Boundary.Requests;

public class CurrencyUpdateRequest
{
  public long Id { get; set; }
  public string Code { get; set; }
  public string Name { get; set; }
}
