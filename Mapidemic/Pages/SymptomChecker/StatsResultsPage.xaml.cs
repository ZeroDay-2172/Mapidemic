namespace Mapidemic.Pages.SymptomChecker;

/// <summary>
/// A class that provides a user interface for the statistical symptom analysis
/// </summary>
public partial class StatsResultsPage : ContentPage
{
    public double GaugeValue { get; set; }

    /// <summary>
    /// The designated constructor for a result page
    /// </summary>
    public StatsResultsPage()
    {
        InitializeComponent();
        BindingContext = MauiProgram.businessLogic;
        if (MauiProgram.businessLogic.ReadSettings().Theme == AppTheme.Dark)
        {
            GaugeLabels.TextColor = Colors.White;
        }
    }

    /// <summary>
    /// A function that updates the gauge based on the current app theme
    /// </summary>
    protected async override void OnAppearing()
    {
        if (Application.Current!.UserAppTheme == AppTheme.Dark) // theme dark -> text white
        {
            GaugeLabels.TextColor = Colors.White;
        }
        else // theme light -> text black
        {
            GaugeLabels.TextColor = Colors.Black;
        }
    }
}