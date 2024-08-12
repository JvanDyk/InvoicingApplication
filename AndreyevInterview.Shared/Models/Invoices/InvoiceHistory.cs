namespace AndreyevInterview.Shared.Models.Invoices;

public class InvoiceHistory : Invoices
{
    public IEnumerable<InvoiceHistoryMessage> LogMessages { get; set; }
}

public class InvoiceHistoryMessage 
{ 
    public string Message { get; set; }
    public DateTime CreatedOn { get; set; }
}
