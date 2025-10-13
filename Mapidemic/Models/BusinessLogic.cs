using System.Collections.ObjectModel;
using System.Text.Json;

namespace Mapidemic.Models;

public class BusinessLogic
{
    private Settings? settings;
    private readonly Database database;
    private const int postalCodeLength = 5;
    private const int probabilityFactor = 100;
    private const string uiSettingsPath = "ui_settings.json";
    public ObservableCollection<Symptom> SymptomList { get; set; }
    public SortedSet<AnalyzedIllness> SymptomAnalysis { get; set; }
    public AnalyzedIllness LikelyIllness { get; set; }

    /// <summary>
    /// The designated constructor for a BusinessLogic
    /// </summary>
    /// <param name="database"></param>
    public BusinessLogic(Database database)
    {
        this.database = database;
        SymptomList = new ObservableCollection<Symptom>();
        LoadSymptomsList();

        /// this function is for testing the settings setup
        // ClearSettings();
        /// comment out this function when not testing

        try
        {
            string jsonSettings = File.ReadAllText(Path.Combine(FileSystem.Current.AppDataDirectory, uiSettingsPath));
            settings = JsonSerializer.Deserialize<Settings>(jsonSettings)!;
        }
        catch (Exception)
        {
            /// null if settings file cannot be read in
            /// or does not exist
            settings = null;
        }
    }

    /// <summary>
    /// A function that loads the local
    /// list of symptoms from the database,
    /// sorts them, and adds them to a collection
    /// </summary>
    private async void LoadSymptomsList()
    {
        SortedSet<string> symptoms = new SortedSet<string>(); ;
        foreach (Illness illness in await database.GetSymptomsList())
        {
            foreach (string symptom in illness.Symptoms!)
            {
                symptoms.Add(symptom);
            }
        }
        foreach (string symptom in symptoms)
        {
            SymptomList.Add(new Symptom(symptom));
        }
    }

    /// <summary>
	/// A helper function that deletes the user's local
	/// ui_settings.json file for developer testing
	/// </summary>
	private void ClearSettings()
    {
        File.Delete(Path.Combine(FileSystem.Current.AppDataDirectory, uiSettingsPath));
    }

    /// <summary>
	/// A function that creates a new ui_settings.json
	/// file in the instance that the file is not present or
	/// has been emptied
	/// </summary>
	public void CreateSettingsFile()
    {
        string destinationPath = Path.Combine(FileSystem.Current.AppDataDirectory, uiSettingsPath);
        var input = FileSystem.OpenAppPackageFileAsync(uiSettingsPath);
        var output = File.Create(destinationPath);
        input.Result.CopyToAsync(output);
    }

    /// <summary>
    /// A function that returns the local Settings
    /// </summary>
    /// <returns>The local settings</returns>
    public Settings ReadSettings()
    {
        return settings!;
    }

    /// <summary>
    /// A function that accepts settings changes and saves
    /// then to the device's local settings file
    /// </summary>
    /// <param name="unitSetting"></param>
    /// <param name="themeEnum"></param>
    /// <param name="postalCode"></param>
    /// <returns>true is settings were updated, false is not</returns>
    public async Task<bool> SaveSettings(bool themeEnum, int postalCode)
    {
        try
        {
            AppTheme enumValue = themeEnum ? AppTheme.Dark : AppTheme.Light;
            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
            string jsonSettings = JsonSerializer.Serialize(new Settings(enumValue, postalCode), options);
            string settingsFile = Path.Combine(FileSystem.Current.AppDataDirectory, uiSettingsPath);
            File.WriteAllText(settingsFile, jsonSettings);
            settings = new Settings(enumValue, postalCode);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// A function that accepts a postal code and
    /// asks the database to validate it
    /// </summary>
    /// <param name="entryText"></param>
    /// <returns>true if valid postal code, false if not</returns>
    public async Task<bool> ValidatePostalCode(string entryText)
    {
        int postalCode;
        bool validPostalCode = true;
        if (entryText == null || entryText.Length != postalCodeLength)
        {
            validPostalCode = false;
        }
        else if (!int.TryParse(entryText, out postalCode))
        {
            validPostalCode = false;
        }
        else
        {
            if (!await database.ValidatePostalCode(postalCode))
            {
                validPostalCode = false;
            }
        }
        return validPostalCode;
    }

    /// <summary>
    /// A function that validates that at least
    /// one checkbox has been used on the symptom
    /// checker page
    /// </summary>
    /// <returns>True if at least one, false if none</returns>
    public async Task<bool> ValidateCheckboxUsed()
    {
        int index = 0;
        while (index < SymptomList.Count)
        {
            if (SymptomList[index].IsChecked)
            {
                return true;
            }
            index++;
        }
        return false;
    }

    /// <summary>
    /// A function that uses a basic probability formula
    /// to determine how likely it is that a user has a specified
    /// illness based on their symptoms
    /// </summary>
    public async void RunSymptomAnalysis()
    {
        SymptomAnalysis = new SortedSet<AnalyzedIllness>(new AnalyzedIllnessComparer());
        HashSet<Symptom> userSymptoms = ProcessCheckedSymptoms();
        List<Illness> illnesses = await database.GetIllnessList();
        foreach (Illness illness in illnesses)
        {
            int matchingSymptoms = 0;
            int extraUserSymptoms = 0;
            int extraIllnessSymptoms;
            double finalProbability;
            HashSet<string> illnessSymptoms = new HashSet<string>(illness.Symptoms!);
            foreach (Symptom symptom in userSymptoms)
            {
                if (illnessSymptoms.Contains(symptom.Name!))
                {
                    matchingSymptoms++;
                }
                else
                {
                    extraUserSymptoms++;
                }
            }
            extraIllnessSymptoms = illnessSymptoms.Count - matchingSymptoms;
            finalProbability = (double)matchingSymptoms / (matchingSymptoms + extraUserSymptoms + extraIllnessSymptoms) * probabilityFactor;
            if (finalProbability != 0)
            {
                SymptomAnalysis.Add(new AnalyzedIllness(illness, finalProbability));
            }
        }
        LikelyIllness = SymptomAnalysis.First();
        SymptomAnalysis.Remove(LikelyIllness);
    }

    /// <summary>
    /// A helper function that creates a set
    /// of all symptoms the user is experiencing
    /// and resets them in the observable collection
    /// for the next symptom check
    /// </summary>
    /// <returns>A set of all the user symptoms</returns>
    private HashSet<Symptom> ProcessCheckedSymptoms()
    {
        HashSet<Symptom> checkedSymptoms = new HashSet<Symptom>();
        foreach (Symptom symptom in SymptomList)
        {
            if (symptom.IsChecked)
            {
                checkedSymptoms.Add(symptom);
                symptom.IsChecked = false;
            }
        }
        return checkedSymptoms;
    }

    public async Task<List<Illnesses>> GetIllnessesList()
    {
        return await database.GetIllnessesList();
    }

    /// <summary>
    /// A function that accepts illness report details
    /// and submits them to the database
    /// </summary>
    /// <param name="id"></param>
    /// <param name="postalCode"></param>
    /// <param name="illness"></param>
    /// <param name="reportDate"></param>
    /// <returns>true if report was a success, false if otherwise</returns>
    public async Task<bool> ReportIllness(Guid id, int postalCode, string illness, DateTimeOffset reportDate)
    {
        var report = new IllnessReport // Create a new illness report object
        {
            Id = id,
            PostalCode = postalCode,
            IllnessType = illness,
            ReportDate = reportDate
        };

        var response = await database.supabaseClient.From<IllnessReport>().Insert(report); // Insert the report into the database

        if (response.ResponseMessage!.IsSuccessStatusCode) // Check if the response indicates success
        {
            return true; // Return true if the insertion was successful
        }
        else
        {
            return false; // Return false if the insertion failed (Might be a bug, while creating this, it still went through)
        }
    }
}