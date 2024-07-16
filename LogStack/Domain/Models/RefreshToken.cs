using System.ComponentModel.DataAnnotations;

namespace LogStack.Domain.Models;

public class RefreshToken
{
    [Key]
    public string Token { get; set; }

    public Ulid UserId { get; set; }
    
    public DateTime CreationDate { get; set; } = DateTime.Now;
    
    public DateTime ExpirationDate { get; set; }
}