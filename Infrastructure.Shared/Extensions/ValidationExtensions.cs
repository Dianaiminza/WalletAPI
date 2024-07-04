using FluentValidation.Results;
using Infrastructure.Shared.CustomExceptions.Models;

namespace Infrastructure.Shared.Extensions;

public static class ValidationExtensions
{
  public static IList<ValidationError> GetAllModelStateErrors(this ValidationResult validationResult)
  {
    return validationResult.Errors
      .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage)).ToList();
  }

  public static IList<string> GetErrorMessageList(this ValidationResult validationResult)
  {
    return validationResult.Errors
      .Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToList();
  }

  public static IDictionary<string, string[]> ToGroupedErrorDictionary(this ValidationResult validationResult)
  {
    return validationResult.Errors
      .GroupBy(x => x.PropertyName)
      .ToDictionary(
        g => g.Key,
        g => g.Select(x => x.ErrorMessage).ToArray()
      );
  }
}
