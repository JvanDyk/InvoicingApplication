using AndreyevInterview.Exceptions;
using Microsoft.AspNetCore.Http;

namespace AndreyevInterview.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        private readonly ExceptionHandler ExceptionHandler;

        public ErrorHandlingMiddleware(ExceptionHandler exceptionHandler)
        {
            ExceptionHandler = exceptionHandler;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await ExceptionHandler.OnExceptionAsync(context, ex);
            }
        }
    }
}
