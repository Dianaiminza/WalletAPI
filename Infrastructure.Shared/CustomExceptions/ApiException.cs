using Infrastructure.Shared.CustomExceptions.Models;
using System.Net;

namespace Infrastructure.Shared.CustomExceptions;

public class ApiException : Exception
{
  public ApiException()
  {
    Errors = Enumerable.Empty<ValidationError>();
  }


  public ApiException(string message,
    HttpStatusCode statusCode = HttpStatusCode.InternalServerError, IEnumerable<ValidationError> errors = null) :
    base(message)
  {
    StatusCode = statusCode;
    Errors = errors;
  }

  public HttpStatusCode StatusCode { get; set; }

  public IEnumerable<ValidationError> Errors { get; set; }
}
