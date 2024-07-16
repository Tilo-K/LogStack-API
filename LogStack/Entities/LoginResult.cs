using LogStack.Domain.Models;

namespace LogStack.Entities;

public class LoginResult
{
    public DateTime IssuingTime { get; set; } = DateTime.Now;

    public string AuthToken { get; set; }
    
    public RefreshToken RefreshToken { get; set; }
}