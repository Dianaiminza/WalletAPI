namespace Infrastructure.Shared.Extensions;

public static class ExceptionExtensions
{
  public static string GetFullMessage(this Exception ex)
  {
    if (ex == null)
    {
      return "Exception message is empty";
    }

    return ex.Message + "; " + ex.InnerException?.GetFullMessage();
  }
}
