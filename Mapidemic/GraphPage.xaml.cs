//Author(s): Connor McGuire

using Microcharts;
using SkiaSharp;
using Microcharts.Maui;

namespace Mapidemic;

public partial class GraphPage : ContentPage
{
    public GraphPage()
    {
        InitializeComponent();

        LoadChart();
    }

    /// <summary>
    /// Function that load's the first chart to display
    /// TBD: MAKE THE CHART DYNAMIC WITH DATA FROM THE DATABASE!
    /// </summary>
    private void LoadChart()
    {
        var entries = new[]
        {
            new ChartEntry(5) { Label = "10/7", ValueLabel = "5,000", Color = SKColor.Parse("#266489") },
            new ChartEntry(8) { Label = "10/8", ValueLabel = "8,000", Color = SKColor.Parse("#68B9C0") },
            new ChartEntry(12) { Label = "10/9", ValueLabel = "12,000", Color = SKColor.Parse("#90D585") },
        };

        chartView.Chart = new LineChart
        {
            Entries = entries,
            LineMode = LineMode.Straight,
            LineSize = 8,
            PointMode = PointMode.Circle,
            PointSize = 18,


            LabelTextSize = 40
        };
    }
}