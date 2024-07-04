using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;
using System.Net.Mime;
using System.Text;

namespace Infrastructure.Shared.Services.HttpClients;

/// <summary>
///   Provides methods to communicate with REST API supporting JSON requests and responses.
/// </summary>
public class JsonRestClient : IRestClient
{
  private HttpClient _httpClient;

  public void Init(HttpClient httpClient)
  {
    _httpClient = httpClient;

    _httpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);
    _httpClient.DefaultRequestHeaders.Add("User-Agent", "NCBA RW BILLERS");
  }

  /// <summary>
  /// Sends a GET method to specified url.
  /// </summary>
  public async Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string> headers = null,
    CancellationToken cancellationToken = default, JsonSerializerSettings jsonSerializerSettings = null)
  {
    return await SubmitRequestAsync(url, null, HttpMethod.Get, headers, cancellationToken, jsonSerializerSettings)
      .ConfigureAwait(false);
  }

  /// <summary>
  /// Sends a POST method to specified url, passing a body in JSON format
  /// </summary>
  public async Task<HttpResponseMessage> PostAsync(string url, object payload,
    Dictionary<string, string> headers = null, CancellationToken cancellationToken = default,
    JsonSerializerSettings jsonSerializerSettings = null)
  {
    return await SubmitRequestAsync(url, payload, HttpMethod.Post, headers, cancellationToken, jsonSerializerSettings)
      .ConfigureAwait(false);
  }

  /// <summary>
  /// Sends a PUT method to specified url, passing a body in JSON format
  /// </summary>
  public async Task<HttpResponseMessage> PutAsync(string url, object payload, Dictionary<string, string> headers = null,
    CancellationToken cancellationToken = default, JsonSerializerSettings jsonSerializerSettings = null)
  {
    return await SubmitRequestAsync(url, payload, HttpMethod.Put, headers, cancellationToken, jsonSerializerSettings)
      .ConfigureAwait(false);
  }

  /// <summary>
  /// Sends a DELETE method to specified url
  /// </summary>
  public async Task<HttpResponseMessage> DeleteAsync(string url, Dictionary<string, string> headers = null,
    CancellationToken cancellationToken = default, JsonSerializerSettings jsonSerializerSettings = null)
  {
    return await SubmitRequestAsync(url, null, HttpMethod.Delete, headers, cancellationToken, jsonSerializerSettings)
      .ConfigureAwait(false);
  }

  private async Task<HttpResponseMessage> SubmitRequestAsync(string url, object payload,
    HttpMethod method, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default,
    JsonSerializerSettings jsonSerializerSettings = null)
  {
    Debug.Assert(_httpClient != null, "Init() method must be called before making any requests");

    var httpRequestMessage = new HttpRequestMessage
    {
      Method = method,
      RequestUri = new Uri($"{_httpClient.BaseAddress}{url}")
    };

    if (headers != null)
    {
      foreach (var (key, value) in headers)
      {
        httpRequestMessage.Headers.Add(key, value);
      }
    }

    if (payload != null)
    {
      jsonSerializerSettings ??= new JsonSerializerSettings
      {
        ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
        Formatting = Formatting.Indented
      };

      var jsonBody = JsonConvert.SerializeObject(payload, jsonSerializerSettings);
      httpRequestMessage.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
    }

    return await _httpClient.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false);
  }
}
