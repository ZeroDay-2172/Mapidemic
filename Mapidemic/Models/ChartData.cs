namespace Mapidemic.Models;

/// <summary>
/// Represents chart data with value and date.
/// </summary>
public class ChartData
{

    public int value { get; set; }
    public DateTimeOffset date { get; set; }

    public string dateString { get; }

    public ChartData(int value, DateTimeOffset date)
    {
        this.value = value;
        this.date = date;
        this.dateString = this.date.ToString("MM/dd");
    }
}