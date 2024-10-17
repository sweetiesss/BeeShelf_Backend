using BeeStore_Repository.Logger.GlobalExceptionHandler.CustomException;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace BeeStore_Repository.Logger.GlobalExceptionHandler
{
    public class GlobalExceptionMiddleware : IMiddleware
    {

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                switch (error)
                {

                    case AppException e:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case DuplicateException e:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case KeyNotFoundException e:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                var result = JsonSerializer.Serialize(new { code = response.StatusCode, message = error?.Message });
                await response.WriteAsync(result);
            }
        }
    }
}
