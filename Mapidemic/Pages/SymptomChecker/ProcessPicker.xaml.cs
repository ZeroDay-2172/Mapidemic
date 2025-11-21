namespace Mapidemic.Pages.SymptomChecker;

public partial class ProcessPickerPage : ContentPage
{
    private readonly SymptomsPage sibling; // holding sibling to pop off

    /// <summary>
    /// The designated constructor for a ProcessPickerPage
    /// </summary>
    public ProcessPickerPage(SymptomsPage sibling)
    {
        InitializeComponent();
        this.sibling = sibling;
    }

    /// <summary>
    /// A function that opens the disclaimer popup
    /// when the page loads
    /// </summary>
    protected override async void OnAppearing()
    {
        Disclaimer.IsOpen = true;
    }

    /// <summary>
    /// A function that shows the user the different symptom
    /// checker options if the user accepted the terms
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public async void AcceptClicked(object sender, EventArgs args)
    {
        Disclaimer.IsOpen = false;
        Options.IsVisible = true;
    }

    /// <summary>
    /// A function that returns the user to the home page
    /// if they did not accept the terms to use the 
    /// symptoms checker
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public async void DeclinedClicked(object sender, EventArgs args)
    {
        Disclaimer.IsOpen = false;
        Popup.IsOpen = true;
        MauiProgram.businessLogic.ClearSymptoms();
        await (Parent as NavigationPage)!.PopToRootAsync();
        Popup.IsOpen = false;
    }

    /// <summary>
    /// A function that directs the user to the artificial
    /// intelligence results page
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public async void OnAiImageTapped(object sender, EventArgs args)
    {
        Popup.IsOpen = true;
        NavigationPage parent = (Parent as NavigationPage)!;
        await MauiProgram.businessLogic.RunAiSymptomAnalysis();
        parent.Navigation.RemovePage(sibling);
        await parent.PushAsync(new AiResultsPage());
        parent.Navigation.RemovePage(this);
        Popup.IsOpen = false;
    }

    /// <summary>
    /// A function that directs the user to the statistics
    /// results page and pops off the previous screens
    /// from the navigation stack
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public async void OnStatsImageTapped(object sender, EventArgs args)
    {
        Popup.IsOpen = true;
        NavigationPage parent = (Parent as NavigationPage)!;
        await MauiProgram.businessLogic.RunStatsSymptomAnalysis();
        parent.Navigation.RemovePage(sibling);
        await parent.PushAsync(new StatsResultsPage());
        parent.Navigation.RemovePage(this);
        Popup.IsOpen = false;
    }
}