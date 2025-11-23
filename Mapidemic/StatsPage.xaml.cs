namespace Mapidemic;

using System;
using System.Linq;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;
using Supabase;
using Mapidemic.Models;
using System.Collections.ObjectModel;
using Syncfusion.Maui.ListView;

public partial class StatsPage : ContentPage
{

    private readonly Dictionary<string, int> _illnesses = new();
    public ObservableCollection<StatData> IllnessItems { get; } = new();

    public StatsPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

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
    private async Task GetReports(Entry sourceEntry = null)
    {
        var entry = sourceEntry ?? this.FindByName<Entry>("ZipEntry");
        var zip = entry?.Text?.Trim();

        // Parse the user's ZIP entry
        if (!int.TryParse(zip, out var postalCode))
        {
            await DisplayAlert("Invalid ZIP", "Please enter a 5-digit ZIP code (e.g., 54901)", "OK");
            return;
        }
        try // attempting database queries
        {
            // Validate if it is a postal code
            if (!await MauiProgram.businessLogic.ValidatePostalCode(zip)) // attempting to validate postal code
            {
                await DisplayAlert("Invalid ZIP", "Please enter a 5-digit ZIP code (e.g., 54901)", "OK");
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
                await DisplayAlert("Network Error", $"{reportError.Message}", "OK");
            }
        }
        catch(Exception postalCodeError) // catching error if the database could not be reached
        {
            await DisplayAlert("Network Error", $"{postalCodeError.Message}", "OK");
        }
    }
        
    private async void Entered_Search(object sender, EventArgs e) => await GetReports(sender as Entry);
    private async void Clicked_Search(object sender, EventArgs e) => await GetReports();
}