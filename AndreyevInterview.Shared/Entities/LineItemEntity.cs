namespace AndreyevInterview.Shared.Entities;

public class LineItemEntity
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public string Description { get; set; }

    public int Quantity { get; set; }
    public decimal Cost { get; set; }
    public bool isBillable { get; set; }

    public InvoiceEntity Invoice { get; set; }
}
