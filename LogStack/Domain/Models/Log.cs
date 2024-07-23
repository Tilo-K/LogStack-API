using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace LogStack.Domain.Models;

[Index(nameof(ProjectId))]
public class Log
{
    [Key]
    public Ulid Id { get; set; } = Ulid.NewUlid();
    
    public Ulid ProjectId { get; set; }

    public string LogLevel { get; set; } = "Information";
    public DateTime Time { get; set; } = DateTime.Now;
    
    public int Day { get; set; } = DateTime.Now.Day;
    
    public int Month { get; set; } = DateTime.Now.Month;
    
    public int Year { get; set; } = DateTime.Now.Year;
    public string Content { get; set; } = "";

    public string Origin { get; set; } = "";
}