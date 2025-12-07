namespace Mapidemic.Pages.Menu;

/// <summary>
/// A class that provides a user interface for displaying the Mapidemic privacy policy
/// </summary>
public partial class PrivacyPolicyPage : ContentPage
{
    /// <summary>
    /// The default constructor for a PrivacyPolicyPage
    /// </summary>
    public PrivacyPolicyPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// A function that controls the toggle status of a section panel
    /// </summary>
    /// <param name="panel"></param>
    private void ToggleAsync(View panel)
    {
        if (panel.IsVisible) // closing the panel
        {
            panel.IsVisible = false;
        }
        else // opening the panel
        {
            panel.IsVisible = true;
        }
    }

    /// <summary>
    /// A function that toggles the intro content panel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ClickedOnIntro(object sender, EventArgs e) => ToggleAsync(IntroContent);

    /// <summary>
    /// A function that toggles the reason content panel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ClickedOnReason(object sender, EventArgs e) => ToggleAsync(ReasonContent);
}