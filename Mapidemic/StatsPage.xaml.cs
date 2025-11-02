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
    private Dictionary<string, Label> _labelByIllness = new();
    public ObservableCollection<string> SymptomHeaderItems { get; } = new();


    public StatsPage()
    {
        InitializeComponent();
        InitIllnessLabelMap();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadSymptomsHeaderAsync(); // fills the header, alphabetically
    }

    private async Task LoadSymptomsHeaderAsync()
    {
        // Pull symptoms from your DB and show them alphabetically
        var set = new SortedSet<string>(StringComparer.CurrentCultureIgnoreCase);

        var illnesses = await MauiProgram.businessLogic.GetIllnessesList();
        if (illnesses != null)
        {
            foreach (var ill in illnesses)
            {
                var s = ill?.Name?.Trim();
                if (!string.IsNullOrWhiteSpace(s))
                    set.Add(s);
            }
        }

        SymptomHeaderItems.Clear();
        foreach (var s in set)
            SymptomHeaderItems.Add(s);

        var headerLabels = new List<Label>
        {
            BronchitisHeaderLabel, ChickenpoxHeaderLabel,
            CommonColdHeaderLabel, CovidHeaderLabel,
            EColiHeaderLabel, InfluenzaHeaderLabel,
            ListeriaHeaderLabel, MeaslesHeaderLabel,
            PneumoniaHeaderLabel, SalmonellaHeaderLabel,
            StrepThroatHeaderLabel, CoughHeaderLabel
        };

        int i = 0;
        foreach (var name in set)
        {
            if (i >= headerLabels.Count) break;
            headerLabels[i].Text = name;
            i++;
        }
    }

    /// <summary>
    /// Initializes the mapping from illness name
    /// to their corresponding label in XAML
    /// </summary>
    private void InitIllnessLabelMap()
    {
        _labelByIllness = new Dictionary<string, Label>()
        {
            ["Bronchitis"] = BronchitisLabel,
            ["Chickenpox"] = ChickenpoxLabel,
            ["Common Cold"] = CommonColdLabel,
            ["COVID-19"] = CovidLabel,
            ["E. Coli"] = EColiLabel,
            ["Influenza"] = InfluenzaLabel,
            ["Listeria"] = ListeriaLabel,
            ["Measles"] = MeaslesLabel,
            ["Pneumonia"] = PneumoniaLabel,
            ["Salmonella"] = SalmonellaLabel,
            ["Strep Throat"] = StrepThroatLabel,
            ["Whooping Cough"] = CoughLabel
        };
    }

    /// <summary>
    /// A function that toggles the button
    /// on and off to display the frame
    /// </summary>
    private async Task ToggleAsync(View panel)
    {
        if (panel.IsVisible)
        {
            await panel.FadeTo(0, 120);
            panel.IsVisible = false;
            panel.Opacity = 1;
        }
        else
        {
            panel.Opacity = 0;
            panel.IsVisible = true;
            await panel.FadeTo(1, 120);
        }
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

        // Update each illness label with its count. 0 if none.
        foreach (var kv in _labelByIllness)
        {
            var illnessName = kv.Key;
            var label = kv.Value;

            if (_illnesses.TryGetValue(illnessName, out var count))
            {
                label.Text = $"{count} user(s) reported of {illnessName} in your area";
            }
            else
            {
                label.Text = $"0 user(s) reported of {illnessName} in your area";
            }
        }
    }
        
    private async void Entered_Search(object sender, EventArgs e) => await GetReports(sender as Entry);
    private async void Clicked_Search(object sender, EventArgs e) => await GetReports();
    private async void Clicked_Bronchitis(object sender, EventArgs e) => await ToggleAsync(BronchitisContent);
    private async void Clicked_Chickenpox(object sender, EventArgs e) => await ToggleAsync(ChickenpoxContent);
    private async void Clicked_CommonCold(object sender, EventArgs e) => await ToggleAsync(CommonColdContent);
    private async void Clicked_Covid(object sender, EventArgs e) => await ToggleAsync(CovidContent);
    private async void Clicked_E_Coli(object sender, EventArgs e) => await ToggleAsync(EColiContent);
    private async void Clicked_Influenza(object sender, EventArgs e) => await ToggleAsync(InfluenzaContent);
    private async void Clicked_Listeria(object sender, EventArgs e) => await ToggleAsync(ListeriaContent);
    private async void Clicked_Measles(object sender, EventArgs e) => await ToggleAsync(MeaslesContent);
    private async void Clicked_Pneumonia(object sender, EventArgs e) => await ToggleAsync(PneumoniaContent);
    private async void Clicked_Salmonella(object sender, EventArgs e) => await ToggleAsync(SalmonellaContent);
    private async void Clicked_StrepThroat(object sender, EventArgs e) => await ToggleAsync(StrepThroatContent);
    private async void Clicked_Cough(object sender, EventArgs e) => await ToggleAsync(CoughContent);

}