namespace AndreyevInterview.Shared.Models;

public class InvoiceHistoryDTO: Invoices
{
    public IEnumerable<string> LogMessages { get; set; }
}