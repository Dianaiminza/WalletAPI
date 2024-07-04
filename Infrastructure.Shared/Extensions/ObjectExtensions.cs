using Infrastructure.Shared.CustomExceptions;
using System.Net;

namespace Infrastructure.Shared.Extensions;

public static class ObjectExtensions
{
  public static T EnsureExists<T>(this T obj, string errorMessage = "Entity not found",
    HttpStatusCode statusCode = HttpStatusCode.NotFound)
  {
    if (obj == null)
    {
      throw new ApiException(errorMessage, statusCode);
    }

    return obj;
  }

  public static async Task<T> EnsureExistsAsync<T>(this Task<T> task, string errorMessage = "Entity not found",
    HttpStatusCode statusCode = HttpStatusCode.NotFound)
  {
    var result = await task.ConfigureAwait(false);
    if (result == null)
    {
      throw new ApiException(errorMessage, statusCode);
    }

    return result;
  }
}
