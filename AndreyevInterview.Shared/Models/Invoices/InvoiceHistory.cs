namespace AndreyevInterview.Shared.Models.Invoices;

public class InvoiceHistory : Invoices
{
    public IEnumerable<string> LogMessages { get; set; }
}