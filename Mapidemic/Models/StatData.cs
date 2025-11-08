namespace Mapidemic.Models;

public class StatData
{
    public string Name { get; set; } = "";
    public int Count { get; set; }
    public string ReportCount => $"# of user(s) reported of {Name} in your area: ";
}