namespace AndreyevInterview.Shared.Models.Invoices;

public class InvoiceModel
{
    public List<Invoices> Invoices { get; set; }
}

public class Invoices
{
    public int Id { get; set; }
    public int Discount { get; set; }
    public string Description { get; set; }
    public decimal TotalValue { get; set; }
    public decimal TotalBillableValue { get; set; }
    public decimal TotalNumberLineItems { get; set; }
}
