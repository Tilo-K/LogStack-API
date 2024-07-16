using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace LogStack.Domain.Models;

[PrimaryKey(nameof(UserId), nameof(ProjectId))]
public class HasAccess
{
    public Ulid UserId { get; set; }
    
    public Ulid ProjectId { get; set; }
}