using System.ComponentModel.DataAnnotations;

namespace LogStack.Domain.Models;

public class TokenSecret
{
    [Key]
    public int Id { get; set; }

    public string Content { get; set; }
}