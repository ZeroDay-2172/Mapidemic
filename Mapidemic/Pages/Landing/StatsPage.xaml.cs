using Mapidemic.Models;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace Mapidemic.Pages.Landing;

/// <summary>
/// A class that provides a user interface for tracking illness on a statistics page
/// </summary>
public partial class StatsPage : ContentPage
{
    private readonly Dictionary<string, int> _illnesses = new();
    public ObservableCollection<StatData> IllnessItems { get; } = new();

    /// <summary>
    /// The designated constructor for a StatsPage
    /// </summary>
    public StatsPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    /// <summary>
    /// A function that gets the reports for the postal code in the settings file
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Get the default ZIP from settings
        int defaultZip = MauiProgram.businessLogic.ReadSettings()?.PostalCode ?? 0;

        // If no default ZIP, exit
        if (defaultZip == 0)
            return;

        // Put it into the ZipEntry
        ZipEntry.Text = defaultZip.ToString();

        // DaysPicked set to 30
        if (DaysPicked.Items.Contains("30"))
            DaysPicked.SelectedItem = "30";

        // Auto-load illnesses for the user's provided default ZIP
        await GetReports(ZipEntry);
    }

    /// <summary>
    /// A method to generate the reports of all illnesses
    /// reports for the postal code and adds them. Then the report
    /// is displayed to it's corresponding labels with the counts.
    /// </summary>
    /// <param name="sourceEntry"></param>
    /// <returns></returns>
    private async Task GetReports(Entry sourceEntry = null)
    {
        Popup.IsOpen = true;
        var entry = sourceEntry ?? this.FindByName<Entry>("ZipEntry");
        var zip = entry?.Text?.Trim();

        // Parse the user's ZIP entry
        if (!int.TryParse(zip, out var postalCode))
        {
            await HomePage.ShowPopup("Please enter a 5-digit ZIP code (e.g., 54901)");
            return;
        }
        try // attempting database queries
        {
            // Validate if it is a postal code
            if (!await MauiProgram.businessLogic.ValidatePostalCode(zip)) // attempting to validate postal code
            {
                await HomePage.ShowPopup("Please enter a 5-digit ZIP code (e.g., 54901)");
                return;
            }
            // Read the days picked from the picker. Default is set to 1 day.
            int days = 1;
            if (DaysPicked?.SelectedItem is string s && int.TryParse(s, out var d))
            {
                days = d;
            }
            try
            {
                // Fetch the counts from businesslogic for the given ZIP and days
                var result = await MauiProgram.businessLogic.GenerateReport(postalCode, days);

                // Fills the dictionary with counts
                _illnesses.Clear();
                foreach (var kv in result)
                {
                    _illnesses[kv.Key] = kv.Value;
                }

                // Fills the Illnessdata with illness that has at least one report sorted alphabetically
                IllnessItems.Clear();
                foreach (var kv in _illnesses.Where(k => k.Value > 0).OrderBy(kv => kv.Key, StringComparer.CurrentCultureIgnoreCase))
                {
                    IllnessItems.Add(new StatData
                    {
                        Name = kv.Key,
                        Count = kv.Value
                    });
                }
            }
            catch(Exception reportError) // catching error if the database could not be reached
            {
                await HomePage.ShowPopup("Unable to load illness statistics");
                Debug.WriteLine(reportError.Message);
            }
        }
        catch(Exception postalCodeError) // catching error if the database could not be reached
        {
            await HomePage.ShowPopup("Unable to load illness statistics");
            Debug.WriteLine(postalCodeError.Message);
        }
        finally
        {
            Popup.IsOpen = false;
        }
    }
    
    /// <summary>
    /// A function that gets the statistics for a given entry
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Entered_Search(object sender, EventArgs e) => await GetReports((sender as Entry)!);

    /// <summary>
    /// A function that gets the statistics for a given postal code entered by the user
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Clicked_Search(object sender, EventArgs e) => await GetReports();
}