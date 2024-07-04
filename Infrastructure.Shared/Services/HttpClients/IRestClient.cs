using Newtonsoft.Json;

namespace Infrastructure.Shared.Services.HttpClients;

/// <summary>
///   Provides methods to communicate with REST API
/// </summary>
public interface IRestClient
{
  /// <summary>
  /// Allows caller to do extra configuration for a given HttpClient instance.
  /// </summary>
  public void Init(HttpClient httpClient);

  /// <summary>
  /// Sends a GET request to specified url
  /// </summary>
  Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default, JsonSerializerSettings jsonSerializerSettings = null);

  /// <summary>
  /// Sends a POST method with payload in request body to specified url
  /// </summary>
  Task<HttpResponseMessage> PostAsync(string url, object payload, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default, JsonSerializerSettings jsonSerializerSettings = null);

  /// <summary>
  /// Sends a PUT request with payload in request body  to specified url.
  /// </summary>
  Task<HttpResponseMessage> PutAsync(string url, object payload, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default, JsonSerializerSettings jsonSerializerSettings = null);

  /// <summary>
  /// Sends a DELETE request to specified url.
  /// </summary>
  Task<HttpResponseMessage> DeleteAsync(string url, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default, JsonSerializerSettings jsonSerializerSettings = null);
}
