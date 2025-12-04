using System.Diagnostics;
using Mapidemic.Pages.Landing;
using System.Collections.ObjectModel;

namespace Mapidemic.Pages.ReportIllness;

/// <summary>
/// A class that provides a user interface for reporting illnesses
/// </summary>
public partial class ReportIllnessPage : ContentPage
{
    private readonly ObservableCollection<string> illnesses = new();

    /// <summary>
    /// The designated constructor for a ReportIllnessPage
    /// </summary>
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
            Busy(true, "Loading illnesses..."); // Show the busy indicator
            var listing = await MauiProgram.businessLogic.GetIllnessesList(); // Get the list of illnesses from the business logic layer
            illnesses.Clear();
            foreach (var listitem in listing)
            {
                if (listitem.Name != null)
                    illnesses.Add(listitem.Name); // Add each illness to the observable collection
            }
        }
        catch (Exception ex)
        {
            await HomePage.ShowPopup("Unable to load illnesses"); // Show an error message if loading fails
            Debug.WriteLine(ex.Message);
        }
        finally { Busy(false); }// Hide the busy indicator
    }

    /// <summary>
    /// Handle the report button click event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnReportButtonClicked(object sender, EventArgs e)
    {
        IllnessError.IsVisible = IllnessPicker.SelectedItem == null; // Ensure an illness is selected
        ZipError.IsVisible = string.IsNullOrWhiteSpace(PostalCodeEntry.Text) || PostalCodeEntry.Text.Length != 5; // Basic ZIP code validation
        if (IllnessError.IsVisible || ZipError.IsVisible)
            return; // Validation failed, do not proceed

        var selectedIllness = IllnessPicker.SelectedItem?.ToString();
        var postalCodeText = PostalCodeEntry.Text?.Trim();

        Busy(true, "Reporting Illness..."); // Show the busy indicator

        try
        {
            if (!await MauiProgram.businessLogic.ValidatePostalCode(postalCodeText!)) 
            {
                ZipError.IsVisible = true;
                Busy(false, "Invalid ZIP Code.");
                return; // Invalid postal code, do not proceed
            }

            Busy(false);
            await Navigation.PushAsync(new ConfirmIllnessPage(selectedIllness!, postalCodeText!)); // Navigate to the confirmation page
        } catch (Exception ex)
        {
            Busy(false);
            await HomePage.ShowPopup("Unable to report illness. Please try again"); // Show an error message if reporting fails
            Debug.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// Show or hide the busy indicator and update the status message
    /// </summary>
    /// <param name="on">True to show the busy indicator, false to hide it</param>
    /// <param name="msg">Optional status message to display</param>
    private void Busy(bool on, string? msg = null)
    {
        BusyIndicator.IsRunning = BusyIndicator.IsVisible = on;
        StatusLabel.Text = on ? msg : "";
    }

}