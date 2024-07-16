using System.ComponentModel.DataAnnotations;

namespace LogStack.Domain.Models;

public class User
{
    [Key]
    public Ulid Id { get; set; } = Ulid.NewUlid();
    
    public string Username { get; set; }
    
    public string Password { get; set; }
    
    public string Email { get; set; }

    public bool Admin { get; set; } = false;

    public DateTime? LastLogin = null;

    public DateTime CreationDate = DateTime.Now;
}