using System.Net;
using BankingSystem.SharedLibrary.Exceptions;
using Flurl.Http;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace BankingSystem.SharedLibrary.Middlewares
{
    public class GlobalExceptionMiddleware : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An unhandled exception occurred.");

            httpContext.Response.ContentType = "application/json";
            string result;
            string? msg;

            switch (exception)
            {
                case ValidationException:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(new { error = exception.Message });
                    msg = "A " + exception.GetType().Name + " exception occurred.";
                    _logger.LogError(exception, msg);
                    break;

                case NotFoundException:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    result = JsonConvert.SerializeObject(new { error = exception.Message });
                    msg = "A " + exception.GetType().Name + " exception occurred.";
                    _logger.LogError(exception, msg);
                    break;

                case JsonException:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(new { error = exception.Message });
                    msg = "A " + exception.GetType().Name + " exception occurred.";
                    _logger.LogError(exception, msg);
                    break;

                case FlurlHttpException:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(new { error = exception.Message });
                    msg = "A " + exception.GetType().Name + " exception occurred.";
                    _logger.LogError(exception, msg);
                    break;

                case MaxAccountException:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(new { error = exception.Message });
                    msg = "A " + exception.GetType().Name + " exception occurred.";
                    _logger.LogError(exception, msg);
                    break;

                case FluentValidation.ValidationException validationException:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    var errors = validationException.Errors.Select(e => e.ErrorMessage).ToList();
                    result = JsonConvert.SerializeObject(new { error = errors });
                    msg = "A " + validationException.GetType().Name + " exception occurred.";
                    _logger.LogError(validationException, msg);
                    break;

                case InsufficentBalanceException:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(new { error = exception.Message });
                    msg = "A " + exception.GetType().Name + " exception occurred.";
                    _logger.LogError(exception, msg);
                    break;

                case BranchNotSetException:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(new { error = exception.Message });
                    msg = "A " + exception.GetType().Name + " exception occurred.";
                    _logger.LogError(exception, msg);
                    break;

                case IncorrectSettingsException:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    result = JsonConvert.SerializeObject(new { error = exception.Message });
                    msg = "A " + exception.GetType().Name + " exception occurred.";
                    _logger.LogError(exception, msg);
                    break;

                case InvalidTypeException:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(new { error = exception.Message });
                    msg = "A " + exception.GetType().Name + " exception occurred.";
                    _logger.LogError(exception, msg);
                    break;

                case MissingInformationException:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(new { error = exception.Message });
                    msg = "A " + exception.GetType().Name + " exception occurred.";
                    // Log error message only for MissingInformationException
                    _logger.LogError(exception, msg);
                    break;

                case DuplicateException:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    result = JsonConvert.SerializeObject(new { error = exception.Message });
                    msg = "A " + exception.GetType().Name + " exception occurred.";
                    _logger.LogError(exception, msg);
                    break;

                case InvalidAmountException:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(new { error = exception.Message });
                    msg = "A " + exception.GetType().Name + " exception occurred.";
                    _logger.LogError(exception, msg);
                    break;

                case UnauthorizedAccessException:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    result = JsonConvert.SerializeObject(new { error = exception.Message });
                    msg = "A " + exception.GetType().Name + " exception occurred.";
                    _logger.LogError(exception, msg);
                    break;

                case InvalidDateException:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(new { error = exception.Message });
                    msg = "A " + exception.GetType().Name + " exception occurred.";
                    _logger.LogError(exception, msg);
                    break;

                default:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(new { error = exception.Message });
                    msg = "A " + exception.GetType().Name + " exception occurred.";
                    _logger.LogError(exception, msg);
                    break;
            }

            await httpContext.Response.WriteAsync(result, cancellationToken);
            return true;
        }
    }
}
