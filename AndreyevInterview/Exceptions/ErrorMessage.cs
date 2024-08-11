namespace AndreyevInterview.Exceptions
{
    class ErrorMessage
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public int Code { get; set; }
        public ErrorMessage()
        {

        }
    }

    enum ErrorCodes
    {
        UnknownError = 1,
        APIUnavailable = 2,
        NotFound = 3,
    }
}
