
using System.Security.Cryptography.X509Certificates;

namespace Mapidemic.Models;

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