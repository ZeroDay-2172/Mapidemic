using System.Collections.ObjectModel;
using System.Text.Json;

namespace Mapidemic.Models;

public class BusinessLogic
{
    private Settings? settings;
    private readonly Database database;
    private const int postalCodeLength = 5;
    private const string uiSettingsPath = "ui_settings.json";
    public ObservableCollection<Symptom> SymptomList { get; set; }

    /// <summary>
    /// The designated constructor for a BusinessLogic
    /// </summary>
    /// <param name="database"></param>
    public BusinessLogic(Database database)
    {
        this.database = database;
        SymptomList = new ObservableCollection<Symptom>();
        LoadSymptomsListAsync();

        /// this function is for testing the settings setup
        ///ClearSettings();
        /// comment out this function when not testing

        try
        {
            string jsonSettings = File.ReadAllText(Path.Combine(FileSystem.Current.AppDataDirectory, uiSettingsPath));
            settings = JsonSerializer.Deserialize<Settings>(jsonSettings)!;
        }
        catch (Exception ex)
        {
            /// null if settings file cannot be read in
            /// or does not exist
            settings = null;
        }
    }

    /// <summary>
    /// A function that loads the local
    /// list of symptoms from the database
    /// </summary>
    private async void LoadSymptomsListAsync()
    {
        foreach (string symptom in await database.GetSymptomsList())
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
        catch (Exception ex)
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
}