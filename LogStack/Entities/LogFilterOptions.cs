namespace LogStack.Entities;

public class LogFilterOptions
{
    public List<string> LogLevels { get; set; } = new List<string>();
    public List<string> Origins { get; set; } = new List<string>();
}