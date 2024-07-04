namespace Infrastructure.Shared.CustomExceptions.Models;

/// <summary>
///   Object that holds a single Validation Error for the business object
/// </summary>
public class ValidationError
{
  public ValidationError(string key, string message)
  {
    Key = key;
    Message = message;
  }

  /// <summary>The name of the field that this error relates to.</summary>
  public string Key { get; set; }

  /// <summary>The error message for this validation error.</summary>
  public string Message { get; set; }
}
