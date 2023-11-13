using Akros.Authorizer.Domain.Entities;
using Akros.Authorizer.Transversal.Common.Exceptions;
using System.Net;

namespace Akros.Autorizer.Services.WebApi.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var errors = new List<string>();
                var message = error.Message;
                var response = context.Response;
                response.ContentType = "application/json";
                errors.Add(message);
                var responseModel = new UserLoggedEntity { Message = "Error al procesar la solicitud", Errors = errors, IsLogged = false };//Response<string>() { Succeeded = false, Msg = error?.Message };

                switch (error)
                {
                    case ApiException e:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                    case ValidationException e:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        responseModel.Errors = e.Errors;
                        break;
                    case KeyNotFoundException e:
                        // not found error
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    default:
                        // unhandled error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }
                var result = System.Text.Json.JsonSerializer.Serialize(responseModel);

                _logger.LogError("StatusCode: {StatusCode} - Detail: {error} - StackTrace {StackTrace}", response.StatusCode, message, error?.StackTrace);

                await response.WriteAsync(result);
            }
        }
    }
}
