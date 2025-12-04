using Mapidemic.Models;
using System.Diagnostics;
using Mapidemic.Pages.Landing;

namespace Mapidemic.Pages.SymptomChecker;

/// <summary>
/// A class that provides a user interface for selecting their symptoms
/// </summary>
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
        if (await MauiProgram.businessLogic.ValidateCheckboxUsed()) // validating at least one symptom is checked
        {
            try // attempting to run symptom analysis
            {
                NavigationPage parent = (Parent as NavigationPage)!;
                await parent.PushAsync(new ProcessPickerPage(this));
            }
            catch (Exception error) // catching error if database could not be reached for symptom analysis
            {
                await HomePage.ShowPopup("Unable to process symptoms. Please try again");
                Debug.WriteLine(error.Message);
            }
        }
        else // no symptoms are checked
        {
            await HomePage.ShowPopup("At least one symptom must be checked");
            (sender as Button)!.IsEnabled = true;
        }
    }
}