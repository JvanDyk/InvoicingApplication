using System.ComponentModel.DataAnnotations;

namespace AndreyevInterview.Shared.Entities;

public class InvoiceHistoryEntity
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Invoice Id is required")]
    public int InvoiceId { get; set; }
    [Required(ErrorMessage = "Log Message is required")]
    [MinLength(2, ErrorMessage = "Log Message too short")]
    public string LogMessage {  get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.Now;

    public InvoiceEntity Invoice { get; set; }
}
