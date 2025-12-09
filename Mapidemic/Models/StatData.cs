namespace Mapidemic.Models;

/// <summary>
/// Represents statistical data with name and count.
/// </summary>
public class StatData
{
    public string Name { get; set; } = "";
    public int Count { get; set; }
    public string ReportCount => $"Number of user(s) reported of {Name.ToLower()} in your area: ";
}