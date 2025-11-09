namespace Mapidemic;

public partial class ResultsPage : ContentPage
{
    public double GaugeValue { get; set; }

    /// <summary>
    /// The designated constructor for a result page
    /// </summary>
    public ResultsPage()
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
        if (!Illness.IsVisible) // not reloading if the user clicked into the settings page
        {
            Popup.IsOpen = true;
        }
        if (Application.Current!.UserAppTheme == AppTheme.Dark)
        {
            GaugeLabels.TextColor = Colors.White;
        }
        else
        {
            GaugeLabels.TextColor = Colors.Black;
        }
    }

    public async void IuClicked(object sender, EventArgs args)
    {
        Popup.IsOpen = false;
        Illness.IsVisible = true;
        Probability.IsVisible = true;
        InformationExpand.IsVisible = true;
        ResultsExpand.IsVisible = true;
    }
}