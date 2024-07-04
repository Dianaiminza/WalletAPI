using Infrastructure.Shared.CustomExceptions;
using Infrastructure.Shared.CustomExceptions.Models;
using Infrastructure.Shared.Extensions;
using Infrastructure.Shared.Serializers;
using Infrastructure.Shared.Wrapper;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;

namespace WalletAPI.Middlewares;

public class ExceptionMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next.Invoke(context).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Debugger.Break();

            var errorMessage = ex.GetFullMessage();
            IResult<IEnumerable<ValidationError>> errorResponse;

            switch (ex)
            {
                case ApiException apiException:
                    errorResponse = Result<IEnumerable<ValidationError>>.Fail(errorMessage,
                        apiException.StatusCode, apiException.Errors);
                    break;

                case ArgumentNullException _:
                    errorResponse = Result<IEnumerable<ValidationError>>.Fail(errorMessage,
                        HttpStatusCode.BadRequest, Enumerable.Empty<ValidationError>());
                    break;

                case ArgumentException _:
                    errorResponse = Result<IEnumerable<ValidationError>>.Fail(errorMessage,
                        HttpStatusCode.BadRequest, Enumerable.Empty<ValidationError>());
                    break;

                case KeyNotFoundException _:
                    errorResponse = Result<IEnumerable<ValidationError>>.Fail(errorMessage,
                        HttpStatusCode.BadRequest, Enumerable.Empty<ValidationError>());
                    break;

                case UnauthorizedAccessException _:
                    errorResponse = Result<IEnumerable<ValidationError>>.Fail("Request not authorized",
                        HttpStatusCode.Unauthorized, Enumerable.Empty<ValidationError>());
                    break;

                default:
                    // unhandled error
                    _logger.LogError($"Unhandled exception: {ex.GetFullMessage()})");
                    errorResponse = Result<IEnumerable<ValidationError>>.Fail(
                        $"An unhandled error occurred: {errorMessage}",
                        HttpStatusCode.InternalServerError, Enumerable.Empty<ValidationError>());
                    break;
            }

            var response = context.Response;
            response.ContentType = "application/problem+json";
            response.StatusCode = (int)errorResponse.StatusCode;
            await response
                .WriteAsync(JsonConvert.SerializeObject(errorResponse, SerializerSettingsHelper.CamelCase()))
                .ConfigureAwait(false);
        }
    }
}
