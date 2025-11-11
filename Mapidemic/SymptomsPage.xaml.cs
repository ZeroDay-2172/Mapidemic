using System.Text;
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
        Popup.IsOpen = true;
        if (await MauiProgram.businessLogic.ValidateCheckboxUsed())
        {
            try // attempting to run symptom analysis
            {
                NavigationPage parent = (Parent as NavigationPage)!;
                await MauiProgram.businessLogic.RunSymptomAnalysis();

                // THIS CODE IS SAMPLE CODE TO ASK CHAT-GPT FOR THE SYMPTOM ANALYSIS
                // StringBuilder symptomString = new StringBuilder();
                // HashSet<Symptom> userSymptoms = MauiProgram.businessLogic.ProcessCheckedSymptoms();
                // foreach (Symptom symptom in userSymptoms)
                // {
                //     symptomString.Append($"{symptom.Name}, ");
                // }
                // await MauiProgram.businessLogic.GetChatResponse(symptomString.ToString());
                // await parent.PushAsync(new BlankPage());

                await parent.PushAsync(new ResultsPage());
                parent.Navigation.RemovePage(this);
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
        Popup.IsOpen = false;
    }
}