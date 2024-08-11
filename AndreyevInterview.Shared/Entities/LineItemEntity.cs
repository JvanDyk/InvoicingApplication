using System.ComponentModel.DataAnnotations;

namespace AndreyevInterview.Shared.Entities;

public class LineItemEntity
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Invoice Id is required")]
    public int InvoiceId { get; set; }


    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; }


    [Required(ErrorMessage = "Quantity is required")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Cost is required")]
    public decimal Cost { get; set; }

    [Required(ErrorMessage = "isBillable is required")]
    public bool isBillable { get; set; }

    public InvoiceEntity Invoice { get; set; }
}
