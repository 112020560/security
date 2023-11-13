using Akros.Authorizer.Application.Common.Exceptions;
using Akros.Authorizer.Application.Features.UserFeatures.Common;
using Microsoft.AspNetCore.Diagnostics;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Text.Json;

namespace Akros.Authorizer.WebApi.Middlewares;

//public class ErrorHandlerMiddleware
//{
//    private readonly RequestDelegate _next;
//    private readonly ILogger<ErrorHandlerMiddleware> _logger;

//    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
//    {
//        _next = next;
//        _logger = logger;
//    }

//    public async Task Invoke(HttpContext context)
//    {
//        try
//        {
//            await _next(context);
//        }
//        catch (Exception error)
//        {
//            var response = context.Response;
//            response.ContentType = "application/json";
//            var responseModel = new ErrorResponse { Islogged = false, Message = error?.Message };

//            switch (error)
//            {
//                case ApiException:
//                    // custom application error
//                    response.StatusCode = (int)HttpStatusCode.BadRequest;
//                    break;
//                //case ValidationException e:
//                //    // custom application error
//                //    response.StatusCode = (int)HttpStatusCode.BadRequest;
//                //    responseModel.Message = string.Join("|", e.Errors);
//                //    break;
//                case KeyNotFoundException:
//                    // not found error
//                    response.StatusCode = (int)HttpStatusCode.NotFound;
//                    break;
//                case UnauthorizedException:
//                    // not found error
//                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
//                    break;
//                case DirectoryException:
//                    // not found error
//                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
//                    break;
//                default:
//                    // unhandled error
//                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
//                    break;
//            }
//            var result = responseModel.ToString(); ;

//            _logger.LogError(error, "[{TransactionId}] - StatusCode: {StatusCode} - Detail: {error} - StackTrace {StackTrace}", context.TraceIdentifier, response.StatusCode, error?.Message, error?.StackTrace);

//            await response.WriteAsync(result);
//        }
//    }
//}
public static class ErrorHandlerExtensions
{
    public static void UseErrorHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature == null) return;

                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.ContentType = "application/json";

                context.Response.StatusCode = contextFeature.Error switch
                {
                    ApiException => (int)HttpStatusCode.InternalServerError,
                    OperationCanceledException => (int)HttpStatusCode.ServiceUnavailable,
                    UnauthorizedException => (int)HttpStatusCode.Unauthorized,
                    DirectoryException => (int)HttpStatusCode.Unauthorized,
                    _ => (int)HttpStatusCode.InternalServerError
                };

                var errorResponse = new ErrorResponse
                {
                    statusCode = context.Response.StatusCode,
                    responseCode = context.Response.StatusCode,
                    Message = contextFeature.Error.GetBaseException().Message,
                    responseMessage = $"No fue posible procesar la solicitud[{context.TraceIdentifier}]",
                    fullErrorMessage = contextFeature.Error.GetBaseException().Message,
                    Islogged = false,
                    loggedIn = DateTime.UtcNow,
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
            });
        });
    }
}
