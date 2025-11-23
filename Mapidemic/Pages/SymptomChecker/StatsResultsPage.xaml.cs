namespace Mapidemic.Pages.SymptomChecker;

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
        Loaded += OnLoaded!;
    }

    public async void OnLoaded(object sender, EventArgs args)
    {
        if (Application.Current!.UserAppTheme == AppTheme.Dark)
        {
            GaugeLabels.TextColor = Colors.White;
        }
        else
        {
            GaugeLabels.TextColor = Colors.Black;
        }
    }
}