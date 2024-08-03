namespace AndreyevInterview.Shared.Models.API;

public class LineItemModel
{
    public List<LineItemEntity> LineItem { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal TotalBillableValue { get; set; }
}
