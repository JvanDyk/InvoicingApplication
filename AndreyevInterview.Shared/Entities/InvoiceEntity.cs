namespace AndreyevInterview.Shared.Entities;

public class InvoiceEntity
{
    public int Id { get; set; }
    public int ClientId { get; set; } = 0;
    public int Discount { get; set; } = 0;
    public string Description { get; set; }
}
