using System.ComponentModel.DataAnnotations;

namespace AndreyevInterview.Shared.Entities;

public class ClientEntity
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Client Name is required")]
    [MinLength(2, ErrorMessage = "Client Name length is too short")]  
    public string Name { get; set; }
    [Required(ErrorMessage = "Client Email is required")]
    [EmailAddress(ErrorMessage = "Client Email is not valid email address")]
    public string Email { get; set; }
    public string? Address { get; set; }
}
