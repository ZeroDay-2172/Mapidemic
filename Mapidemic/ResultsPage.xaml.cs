namespace Mapidemic;

public partial class ResultsPage : ContentPage
{
    /// <summary>
    /// The designated constructor for a
    /// result page
    /// </summary>
    public ResultsPage()
    {
        InitializeComponent();
        BindingContext = MauiProgram.businessLogic;
        if (MauiProgram.businessLogic.ReadSettings().Theme == AppTheme.Dark)
        {
            GaugeLabels.TextColor = Colors.White;
        }
    }
}