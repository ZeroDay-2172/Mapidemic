using System.Text.Json;
using System.Collections.ObjectModel;

namespace Mapidemic.Models;

public class BusinessLogic
{
    private Settings? settings;
    private Database database;
    private const int postalCodeLength = 5;
    private const int probabilityFactor = 100;
    private const string uiSettingsPath = "ui_settings.json";
    public ObservableCollection<Symptom> SymptomList { get; set; }
    public SortedSet<AnalyzedIllness>? SymptomAnalysis { get; set; }
    public AnalyzedIllness? LikelyIllness { get; set; }

    /// <summary>
    /// The designated constructor for a BusinessLogic
    /// </summary>
    /// <param name="database"></param>
    public BusinessLogic(Database database)
    {
        // this function is for testing the settings setup
        // ClearSettings();
        // comment out this function when not testing

        LikelyIllness = null;
        SymptomAnalysis = null;
        this.database = database;
        SymptomList = new ObservableCollection<Symptom>();
        try // attempting to load local settings file
        {
            string jsonSettings = File.ReadAllText(Path.Combine(FileSystem.Current.AppDataDirectory, uiSettingsPath));
            settings = JsonSerializer.Deserialize<Settings>(jsonSettings)!;
        }
        catch (Exception) // setting settings to null if they don't exist to force app setup
        {
            settings = null;
        }
    }

    /// <summary>
    /// A function that asks the database to test the database connection
    /// </summary>
    /// <returns>True is connection valid, false if not</returns>
    public async Task<bool> TestDatabaseConnection()
    {
        return await database.TestConnection();
    }

    /// <summary>
    /// A function that loads the local
    /// list of symptoms from the database,
    /// sorts them, and adds them to a collection
    /// </summary>
    public async Task<bool> LoadSymptomsList()
    {
        try // attempting to read the symptoms list
        {
            if (SymptomList.Count == 0) // only reading from the database once
            {
                SortedSet<string> symptoms = new SortedSet<string>(); // auxiliary storage for sorting
                foreach (Illness illness in await database!.GetSymptomsList()) // getting full illness list
                {
                    foreach (string symptom in illness.Symptoms!) // storing each unique illness
                    {
                        symptoms.Add(symptom);
                    }
                }
                foreach (string symptom in symptoms) // adding each unique illness to an observable collection for live updates
                {
                    SymptomList!.Add(new Symptom(symptom));
                }
            }
            return true;
        }
        catch(Exception error) // error if the database could not be reached
        {
            throw new Exception(error.Message);
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
    /// <param name="themeEnum"></param>
    /// <param name="postalCode"></param>
    /// <returns>true is settings were updated, false is not</returns>
    public async Task<bool> SaveSettings(bool themeEnum, int postalCode)
    {
        try // attempting to save param settings
        {
            AppTheme enumValue = themeEnum ? AppTheme.Dark : AppTheme.Light;
            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
            string jsonSettings = JsonSerializer.Serialize(new Settings(enumValue, postalCode), options);
            string settingsFile = Path.Combine(FileSystem.Current.AppDataDirectory, uiSettingsPath);
            File.WriteAllText(settingsFile, jsonSettings);
            settings = new Settings(enumValue, postalCode);
            return true;
        }
        catch (Exception) // catching json parse error
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
        if (entryText == null || entryText.Length != postalCodeLength) // checking for proper 5-digit length
        {
            validPostalCode = false;
        }
        else if (!int.TryParse(entryText, out postalCode)) // checking for integer value (U.S. postal codes only)
        {
            validPostalCode = false;
        }
        else // validating postal code
        {
            try // attempting to communicate with the database
            {
                if (!await database.ValidatePostalCode(postalCode)) // checking for invalid postal codes
                {
                    validPostalCode = false;
                }
            }
            catch(Exception error) // passing the exception message to the ui-layer if the database cannot be reached
            {
                throw new Exception(error.Message);
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
        while (index < SymptomList.Count) // checking all checkboxes
        {
            if (SymptomList[index].IsChecked) // verifying if a checkbox is checked
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
    /// <returns>true if the symptom analysis completed</returns>
    public async Task<bool> RunSymptomAnalysis()
    {
        SymptomAnalysis = new SortedSet<AnalyzedIllness>(new AnalyzedIllnessComparer());
        HashSet<Symptom> userSymptoms = ProcessCheckedSymptoms(); // getting all user symptoms
        try // attempting to read the illness list from the database
        {
            List<Illness> illnesses = await GetIllnessesList(); // getting a list of all illnesses
            foreach (Illness illness in illnesses) // checking each illness
            {
                int matchingSymptoms = 0;
                int extraUserSymptoms = 0;
                int extraIllnessSymptoms;
                double finalProbability;
                HashSet<string> illnessSymptoms = new HashSet<string>(illness.Symptoms!); // transforming list => set for constant comparisons
                foreach (Symptom symptom in userSymptoms) // checking each user illness
                {
                    if (illnessSymptoms.Contains(symptom.Name!)) // checking if the user symptoms is an illness symptom
                    {
                        matchingSymptoms++;
                    }
                    else
                    {
                        extraUserSymptoms++;
                    }
                }
                extraIllnessSymptoms = illnessSymptoms.Count - matchingSymptoms; // performing probability calculation
                finalProbability = (double)matchingSymptoms / (matchingSymptoms + extraUserSymptoms + extraIllnessSymptoms) * probabilityFactor;
                if (finalProbability != 0) // ignoring illnesses that do not match the user symptoms
                {
                    SymptomAnalysis.Add(new AnalyzedIllness(illness, finalProbability));
                }
            }
            LikelyIllness = SymptomAnalysis.First(); // extracting the likely illness
            SymptomAnalysis.Remove(LikelyIllness);
            return true;
        }
        catch (Exception error) // catching error if database could not be reached
        {
            throw new Exception(error.Message);
        }
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
        HashSet<Symptom> checkedSymptoms = new HashSet<Symptom>(); // structure to hold user symptoms, set for constant insertion
        foreach (Symptom symptom in SymptomList) //  getting each symptom in the list
        {
            if (symptom.IsChecked) // adding to checked symptoms
            {
                checkedSymptoms.Add(symptom);
                symptom.IsChecked = false;
            }
        }
        return checkedSymptoms;
    }

    /// <summary>
    /// A function that return all the illnesses
    /// </summary>
    /// <returns>a list of all illnesses</returns>
    public async Task<List<Illness>> GetIllnessesList()
    {
        try // attempting to read the illness list from the database
        {
            return await database.GetIllnessesList();
        }
        catch (Exception error) // throwing error for ui if database could not be reached
        {
            throw new Exception(error.Message);
        }
    }

    /// <summary>
    /// A function that gets a list containing each postal
    /// code that has one or more illness reports
    /// </summary>
    /// <returns>a list of postal codes and illness reports count values</returns>
    public async Task<List<ZipIllnessCounts>> GetZipIllnessCounts()
    {
        try // attempting to read the postal code and illness reports counts list
        {
            return await database.GetZipIllnessCounts();
        }
        catch(Exception error) // throwing an error for ui if the database could not be reached
        {
            throw new Exception(error.Message);
        }
    }

    /// <summary>
    /// A function that gets the centroid values on the map
    /// for each postal code in the United States
    /// </summary>
    /// <param name="postalCode"></param>
    /// <returns>a list containing all the centroid values</returns>
    public async Task<List<PostalCodeCentroids>> GetPostalCodeCentroids(int postalCode)
    {
        try // attempting to read the postal code centroid list
        {
            return await database.GetPostalCodeCentroids(postalCode);
        }
        catch(Exception error) // throwing an error for ui if the database could not be reached
        {
            throw new Exception(error.Message);
        }
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

    /// <summary>
    /// Function calls database to return number of cases of specific illness on
    /// given day either locally or nationally.
    /// </summary>
    /// <param name="selectedIllness">name of illness</param>
    /// <param name="date">date to get data for</param>
    /// <param name="localTrends">local to zip code (true), or national (false)</param>
    /// <returns></returns>
    public async Task<int> getNumberOfReports(string selectedIllness, DateTimeOffset date, bool localTrends)
    {
        try // attempting to get total illness reports from the database
        {
            if (localTrends == true)
                return await database.getNumberOfReports(selectedIllness, date, ReadSettings().PostalCode);
            else
                return await database.getNumberOfReports(selectedIllness, date, -1);
        }
        catch (Exception error) // catching error if the database could not be reached
        {
            throw new Exception(error.Message);
        }
    }

    /// <summary>
    /// A function that generates illness count
    /// for a postal code over the past days
    /// </summary>
    /// <param name="postalCode"></param>
    /// <param name="daysPicked"></param>
    /// <returns>a dictionary containing all illness based on the postal code</returns>
    public async Task<Dictionary<string, int>> GenerateReport(int postalCode, int daysPicked)
    {
        try // attempting to read illness report from the database
        {
            // Fetch from the database
            var report = await database.GenerateReport(postalCode, daysPicked);

            // Creates a new count per illness
            var counts = new Dictionary<string, int>();

            // Add counts for each reported illness
            foreach (var r in report)
            {
                if (counts.TryGetValue(r.IllnessType, out var n))
                {
                    counts[r.IllnessType] = n + 1;
                }
                else
                {
                    counts[r.IllnessType] = 1;
                }
            }
            return counts;
        }
        catch(Exception error) // catching error if the database could not be reached
        {
            throw new Exception(error.Message);
        }
    }
}