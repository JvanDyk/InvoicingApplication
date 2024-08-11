namespace AndreyevInterview.Shared.Models.Invoices;

public class LineItemModel
{
    public List<LineItemEntity> LineItem { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal TotalBillableValue { get; set; }
}
