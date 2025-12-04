namespace Mapidemic.Pages.Landing;

/// <summary>
/// A class that provides a user interface for the different methods of illness tracking
/// </summary>
public partial class TrackingPage : TabbedPage
{
    /// <summary>
    /// The default constructor for a TrackingPage
    /// </summary>
    public TrackingPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Function sets it so that upon loading, the theme is updated.
    /// This is due to themes not being applied to the tabs of a 
    /// TabbedPage without entirely recreating the page.
    /// </summary>
    protected override void OnAppearing()
    {
        //Default behavior
        base.OnAppearing();

        //New behavior
        var par = this.Parent.Parent as HomePage;
        par!.NotInSettings();

        if (Application.Current!.UserAppTheme == AppTheme.Dark)
        {
            this.BarBackgroundColor = Colors.Black;
            this.BarTextColor = Colors.White;
        }
        else
        {
            this.BarBackgroundColor = Colors.White;
            this.BarTextColor = Colors.Black;
        }
    }
}