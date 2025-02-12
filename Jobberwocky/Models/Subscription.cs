using System.ComponentModel.DataAnnotations;

namespace Jobberwocky.Models;
public class Subscription
{
    [Required]
    public string Email { get; set; }

    public string? Filter {  get; set; }
}
