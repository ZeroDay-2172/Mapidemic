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
        /// await DisplayAlert("", $"{(sender as Label)!.BindingContext}", "OK");
        Symptom symptom = ((sender as Label)!.BindingContext as Symptom)!;
        symptom.IsChecked = !symptom.IsChecked;
        CheckBox checkBox = (((sender as Label)!.Parent as Grid)!.Children.First() as CheckBox)!;
        checkBox.IsChecked = !checkBox.IsChecked;
    }

    /// <summary>
    /// A function that collects all of the symptoms
    /// that were clicked by the user and passes them
    /// to the results page
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void OnEnterClicked(object sender, EventArgs args)
    {
        HashSet<string> userSymptoms = new HashSet<string>();
        foreach (Symptom symptom in MauiProgram.businessLogic.SymptomList)
        {
            if (symptom.IsChecked)
            {
                /// START HERE TOMORROW. ADD NEW WINDOW FOR NEXT PAGE
                userSymptoms.Add(symptom.Name!);
            }
        }
    }
}