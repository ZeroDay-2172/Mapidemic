namespace Mapidemic;

public partial class LoadingPage : ContentPage
{
    /// <summary>
    /// The designated constructor for a
    /// Loading page
    /// </summary>
    public LoadingPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// A function that starts the second
    /// load bar and creates a results page
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void DataGathered(object sender, EventArgs args)
    {
        /// not proceeding if user clicked off screen
        if (Parent.Parent != null)
        {
            AnalyzingBar.FadeTo(1, 500);
            Analyzing.FadeTo(1, 500);
            AnalyzingBar.Progress = 100;
        }
    }

    /// <summary>
    /// A function that starts the third
    /// load bar and inserts the result page
    /// for a pop can occur that will send
    /// the user to the result page
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void AnalyzingComplete(object sender, EventArgs args)
    {
        /// not proceeding if user clicked off screen
        if (Parent.Parent != null)
        {
            MatchingBar.FadeTo(1, 500);
            Matching.FadeTo(1, 500);
            MatchingBar.Progress = 100;
        }
    }

    /// <summary>
    /// A function that pops this page off the
    /// navigation stack and sets the title
    /// for the results page
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public async void LoadingComplete(object sender, EventArgs args)
    {
        /// not proceeding if user clicked off screen
        if (Parent.Parent != null)
        {
            NavigationPage parentPage = (Parent.Parent as NavigationPage)!;
            NavigationPage resultPage = new NavigationPage(new ResultsPage());
            resultPage.BarTextColor = Color.FromArgb("#FFFFFF");
            resultPage.BarBackgroundColor = Color.FromArgb("#0074C0");
            _ = parentPage.PopAsync();
            await parentPage.PushAsync(resultPage);
        }
    }
}