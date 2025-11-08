namespace Mapidemic;

using System;
using System.Linq;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;
using Supabase;
using Mapidemic.Models;
using System.Collections.ObjectModel;

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

        // Validate if it is a postal code
        bool isValid = await MauiProgram.businessLogic.ValidatePostalCode(zip);
        if (!isValid)
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

        // Fetch the counts from businesslogic for the given ZIP and days
        var result = await MauiProgram.businessLogic.GenerateReport(postalCode, days);

        // Fills the dictionary with counts
        _illnesses.Clear();
        foreach (var kv in result)
        {
            _illnesses[kv.Key] = kv.Value;
        }

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
        
    private async void Entered_Search(object sender, EventArgs e) => await GetReports(sender as Entry);
    private async void Clicked_Search(object sender, EventArgs e) => await GetReports();
}