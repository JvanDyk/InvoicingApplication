using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace AndreyevInterview.Exceptions;

public class ExceptionHandler
{
    private readonly ILogger _logger;
    private readonly IWebHostEnvironment HostingEnvironment;

    public ExceptionHandler(ILogger<ExceptionHandler> logger, IWebHostEnvironment hostingEnvironment)
    {
        _logger = logger;
        HostingEnvironment = hostingEnvironment;
    }

    public async Task OnExceptionAsync(HttpContext context, Exception exception)
    {
        LogException(context, exception);

        var errorMessage = CreateErrorMessage(exception);

        context.Response.StatusCode = errorMessage.StatusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonConvert.SerializeObject(errorMessage));
    }

    private ErrorMessage CreateErrorMessage(Exception exception)
    {
        string message;
        ErrorCodes code = ErrorCodes.UnknownError;
        int status = (int)HttpStatusCode.InternalServerError;

        var exceptionResult = exception switch
        {
            NotFoundException _ => (exception.Message, ErrorCodes.NotFound, (int)HttpStatusCode.NotFound),
            // Add other custom exception handling as needed
            _ => (exception.Message, ErrorCodes.UnknownError, (int)HttpStatusCode.InternalServerError)
        };
       
        (message, code, status) = exceptionResult;

        // If you want to hide errors from the public
        //  if (HostingEnvironment.IsProduction() || HostingEnvironment.IsEnvironment("prd"))
        //  {
        //      message = "Something went wrong. Internal server error";
        //  }

        return new ErrorMessage()
        {
            StatusCode = status,
            Code = (int)code,
            Message = message
        };
    }

    /// <summary>
    /// Logs the exception.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="exception"></param>
    private void LogException(HttpContext context, Exception exception)
    {
        var request = context.Request;
        var logDetails = new
        {
            Exception = exception,
            TraceIdentifier = context.TraceIdentifier,
            HttpMethod = request.Method,
            RequestPath = request.Path,
            QueryString = request.QueryString.ToString(),
            BodyContent = ReadRequestBody(request),
            Headers = request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
            Source = exception.TargetSite?.DeclaringType?.FullName
        };

        _logger.LogError(exception, "Exception occurred: {@LogDetails}", logDetails);
    }

    /// <summary>
    /// Reads the request body.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns></returns>
    private string ReadRequestBody(HttpRequest request)
    {
        request.EnableBuffering();
        var buffer = new byte[Convert.ToInt32(request.ContentLength)];
        request.Body.Read(buffer, 0, buffer.Length);
        var bodyAsText = Encoding.UTF8.GetString(buffer);
        request.Body.Position = 0;
        return bodyAsText;
    }
}
