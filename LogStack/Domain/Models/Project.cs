using System.ComponentModel.DataAnnotations;

namespace LogStack.Domain.Models;

public class Project
{
    [Key] public Ulid Id { get; set; } = Ulid.NewUlid();
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string Secret { get; set; }

    [Required] public DateTime CreationDate { get; set; } = DateTime.Now;
}