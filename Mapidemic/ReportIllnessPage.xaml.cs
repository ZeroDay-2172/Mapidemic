using Supabase;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel;
using System.Collections.ObjectModel;
using Mapidemic.Models;

namespace Mapidemic;

public partial class ReportIllnessPage : ContentPage
{
    private readonly ObservableCollection<string> illnesses = new();
    private readonly BusinessLogic BusinessLogic = new(new Database());
    public ReportIllnessPage()
    {
        InitializeComponent();
        IllnessPicker.ItemsSource = illnesses;
    }

    /// <summary>
    /// Load illnesses from the database when the page appears
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadIllnessesAsync();
    }

    /// <summary>
    /// Load illnesses from the database and populate the picker
    /// </summary>
    private async Task LoadIllnessesAsync()
    {
        try
        {
            var listing = await BusinessLogic.GetIllnessesList(); // Get the list of illnesses from the business logic layer
            illnesses.Clear(); // Clear the existing illnesses
            foreach (var listitem in listing)
            {
                if (listitem.Illness != null)
                    illnesses.Add(listitem.Illness); // Add each illness to the observable collection
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load illnesses: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Handle the report button click event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnReportButtonClicked(object sender, EventArgs e)
    {
        if (IllnessPicker.SelectedItem == null)
        {
            await DisplayAlert("Error", "Please select an illness.", "OK"); // If the user hasn't selected an illness, show an error message
            return;
        }
        string? selectedIllness = IllnessPicker.SelectedItem.ToString(); // Otherwise, get the selected illness

        string postalCodeText = PostalCodeEntry.Text; // Get the postal code from the entry
        if (!await BusinessLogic.ValidatePostalCode(postalCodeText))
        {
            await DisplayAlert("Error", "Please enter a valid postal code.", "OK"); // If the postal code is invalid, show an error message
            return;
        }

        int postalCode = int.Parse(postalCodeText); // Parse the postal code to an integer
        Guid reportId = Guid.NewGuid(); // Generate a new GUID for the report
        DateTimeOffset reportDate = DateTimeOffset.UtcNow; // Get the current date and time in UTC
        bool success = await BusinessLogic.ReportIllness(reportId, postalCode, selectedIllness!, reportDate); // Report the illness using the business logic layer
        if (success)
        {
            await DisplayAlert("Success", "Illness reported successfully.", "OK"); // If the report was successful, show a success message
            IllnessPicker.SelectedItem = null;
            PostalCodeEntry.Text = string.Empty; // Clear the input fields
        }
        else
        {
            await DisplayAlert("Error", "Failed to report illness. Please try again.", "OK"); // If the report failed, show an error message (This should not happen in normal circumstances and might still go through)
        }
    }

}