namespace Mapidemic.Pages.Menu;

/// <summary>
/// A class that provides a user interface for displaying information on the Mapidemic team
/// </summary>
public partial class MeetTheTeamPage : TabbedPage
{
    /// <summary>
    /// The default constructor for a MeetTheTeamPage
    /// </summary>
    public MeetTheTeamPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Opens the default email client to contact Neng Yang
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void NengEmailClicked(object sender, EventArgs e)
    {
        try
        {
            await Launcher.OpenAsync("mailto:yangne53@uwosh.edu");
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Unable to open email client.", "OK");
        }
    }

    /// <summary>
    /// Opens Neng Yang's LinkedIn profile in the default web browser
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void NengLinkedInClicked(object sender, EventArgs e)
    {
        try
        {
            var uri = new Uri("https://www.linkedin.com/in/nengyang93/");
            await Launcher.OpenAsync(uri);
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Unable to open LinkedIn profile.", "OK");
        }
    }
}