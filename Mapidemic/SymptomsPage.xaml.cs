using Mapidemic.Models;

namespace Mapidemic;

public partial class SymptomsPage : ContentPage
{
    /// <summary>
    /// The designated constructor for the Symptoms page
    /// </summary>
    public SymptomsPage()
    {
        InitializeComponent();
        BindingContext = MauiProgram.businessLogic;
    }

    /// <summary>
    /// A function that extends the click range
    /// of the check boxes to cover the total
    /// area of the grid they occupy
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void OnSymptomTapped(object sender, EventArgs args)
    {
        Symptom symptom = ((sender as Label)!.BindingContext as Symptom)!;
        symptom.IsChecked = !symptom.IsChecked;
        CheckBox checkBox = (((sender as Label)!.Parent as Grid)!.Children.First() as CheckBox)!;
        checkBox.IsChecked = !checkBox.IsChecked;
    }

    /// <summary>
    /// A function that tells the business logic
    /// to being analyzing symptoms while the
    /// symptoms page sets up a loading screen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public async void OnEnterClicked(object sender, EventArgs args)
    {
        (sender as Button)!.IsEnabled = false;
        if (await MauiProgram.businessLogic.ValidateCheckboxUsed())
        {
            try // attempting to run symptom analysis
            {
                await MauiProgram.businessLogic.RunSymptomAnalysis();
                NavigationPage parentPage = (Parent.Parent as NavigationPage)!;
                NavigationPage newPage = new NavigationPage(new LoadingPage());
                newPage.BarTextColor = Color.FromArgb("#FFFFFF");
                newPage.BarBackgroundColor = Color.FromArgb("#0074C0");
                _ = parentPage.PopAsync();
                await parentPage.PushAsync(newPage);
            }
            catch (Exception error) // catching error if database could not be reached for symptom analysis
            {
                await DisplayAlert("Network Error", $"{error.Message}", "OK");
            }
        }
        else
        {
            await DisplayAlert(null, "At least one symptom must be checked", "OK");
            (sender as Button)!.IsEnabled = true;
        }
    }
}