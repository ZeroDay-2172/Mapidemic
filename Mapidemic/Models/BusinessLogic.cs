using System.Text.Json;

namespace Mapidemic.Models;

public class BusinessLogic
{
    private const string uiSettingsPath = "ui_settings.json";
    private Database database;
    private const int postalCodeLength = 5;

    /// <summary>
    /// The designated constructor for a BusinessLogic
    /// </summary>
    /// <param name="database"></param>
    public BusinessLogic(Database database)
    {
        this.database = database;
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