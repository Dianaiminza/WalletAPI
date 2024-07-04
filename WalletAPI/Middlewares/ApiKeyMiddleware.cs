
using Domain.Configuration;
using Infrastructure.Shared.CustomExceptions;
using Microsoft.Extensions.Options;
using System.Net;

namespace WalletAPI.Middlewares;

public class ApiKeyMiddleware
{
    private const string ApiKey = "x-api-key";
    private readonly RequestDelegate _requestDelegate;
    private readonly SecurityConfiguration _securityConfigOptions;

    public ApiKeyMiddleware(IOptions<SecurityConfiguration> securityConfigOptions, RequestDelegate requestDelegate)
    {
        _securityConfigOptions = securityConfigOptions.Value;
        _requestDelegate = requestDelegate;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (!httpContext.Request.Headers.TryGetValue(ApiKey, out var apiValue))
            throw new ApiException("API key was not provided", HttpStatusCode.Unauthorized);

        var apiKey = _securityConfigOptions.ApiKey;
        if (!apiKey.Equals(apiValue)) throw new ApiException("Invalid API Key", HttpStatusCode.Forbidden);

        await _requestDelegate(httpContext);
    }
}
