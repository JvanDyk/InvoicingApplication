using System.ComponentModel.DataAnnotations;

namespace AndreyevInterview.Shared.Entities;

public class InvoiceEntity
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Client Id is required")]
    public int ClientId { get; set; }
    public int Discount { get; set; } = 0;
    public string Description { get; set; }
}
