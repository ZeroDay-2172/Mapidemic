namespace Mapidemic.Pages.SymptomChecker;

/// <summary>
/// A class that provides a user interface for the AI symptom analysis
/// </summary>
public partial class AiResultsPage : ContentPage
{
    private bool drawingStarted;
    private const int typeDelay = 35;
    private const int intTypeDelay = 5;
    private const int initialDelay = 500;
    private const string identifiedIllness = "Identified Illness";
    private const string gemini = "Powered by Google Gemini";
    private const string average = "Average time in hours and days";
    private const string hours = "Hour(s):";
    private const string days = "Days(s):";
    private const string division = " / ";
    private const string contagiousPeriod = "Contagious Period";
    private const string recoveryPeriod = "Recovery Period";
    private const string fullSymptomList = "Full Symptom List";
    private const string cdc = "Obtained from the Center for Disease Control and Prevention";

    /// <summary>
    /// The designated constructor of an AiResultsPage
    /// </summary>
    public AiResultsPage()
    {
        InitializeComponent();
        drawingStarted = false;
    }

    /// <summary>
    /// A function that begins the drawing process of the illness information
    /// </summary>
    protected override async void OnAppearing()
    {
        if (Application.Current!.UserAppTheme == AppTheme.Dark) // making the border outlines white on dark theme
        {
            Illness.Stroke = Colors.White;
            ContagiousPeriod.Stroke = Colors.White;
            RecoveryPeriod.Stroke = Colors.White;
            Symptoms.Stroke = Colors.White;
        }
        else // making the border outlines black on light theme
        {
            Illness.Stroke = Colors.Black;
            ContagiousPeriod.Stroke = Colors.Black;
            RecoveryPeriod.Stroke = Colors.Black;
            Symptoms.Stroke = Colors.Black;
        }
        if (!drawingStarted) // drawing the illness sections only once
        {
            drawingStarted = true;
            await Task.Delay(initialDelay);
            await DrawIdentifiedIllness();
            await Task.WhenAll(DrawContagiousPeriod(), DrawRecoveryPeriod());
            await DrawSymptomList();
        }
    }

    /// <summary>
    /// A function that draws the identified illness section of the page and
    /// lets the page know if it should continue drawing after
    /// </summary>
    private async Task<bool> DrawIdentifiedIllness()
    {
        Illness.IsVisible = true;
        foreach(char letter in identifiedIllness.ToCharArray()) // printing header
        {
            IdentifiedIllness.Text = $"{IdentifiedIllness.Text}{letter}";
            await Task.Delay(typeDelay);
        }
        foreach(char letter in gemini.ToCharArray()) // printing credit
        {
            GeminiMessage.Text = $"{GeminiMessage.Text}{letter}";
            await Task.Delay(typeDelay);
        }
        string illness = MauiProgram.businessLogic.artificialIntelligenceIllness!.Name!;
        if (illness == "Flu")
        {
            illness = "Influenza";
        }
        foreach(char letter in illness.ToCharArray()) // printing illness
        {
            IllnessName.Text = $"{IllnessName.Text}{letter}";
            await Task.Delay(typeDelay);
        }
        return this.IsLoaded;
    }

    /// <summary>
    /// A function that draws the contagious period section of the page and
    /// lets the page know if it should continue drawing after
    /// </summary>
    private async Task<bool> DrawContagiousPeriod()
    {
        ContagiousPeriod.IsVisible = true;
        foreach(char letter in contagiousPeriod.ToCharArray()) // printing header
        {
            ContagiousHeader.Text = $"{ContagiousHeader.Text}{letter}";
            await Task.Delay(typeDelay);
        }
        foreach(char letter in average.ToCharArray()) // printing average message
        {
            ContagiousAverage.Text = $"{ContagiousAverage.Text}{letter}";
            await Task.Delay(typeDelay);
        }
        foreach(char letter in hours.ToCharArray()) // printing contagious hours text
        {
            ContagiousHoursText.Text = $"{ContagiousHoursText.Text}{letter}";
            await Task.Delay(typeDelay);
        }
        int numHours = MauiProgram.businessLogic.artificialIntelligenceIllness!.ContagiousPeriod;
        ContagiousHoursNumber.Text = "1";
        for(int value = 1; value < numHours; value++) // printing contagious hours
        {
            ContagiousHoursNumber.Text = $"{int.Parse(ContagiousHoursNumber.Text) + 1}";
            await Task.Delay(intTypeDelay);
        }
        foreach(char letter in days.ToCharArray()) // printing contagious days text
        {
            ContagiousDaysText.Text = $"{ContagiousDaysText.Text}{letter}";
            await Task.Delay(typeDelay);
        }
        foreach(char number in ContagiousHoursNumber.Text.ToCharArray()) // printing contagious days number
        {
            ContagiousDaysNumber.Text = $"{ContagiousDaysNumber.Text}{number}";
            await Task.Delay(typeDelay);
        }
        foreach(char letter in division.ToCharArray())
        {
            ContagiousDaysNumber.Text = $"{ContagiousDaysNumber.Text}{letter}";
            await Task.Delay(typeDelay);
        }
        foreach(char number in "24".ToCharArray())
        {
            ContagiousDaysNumber.Text = $"{ContagiousDaysNumber.Text}{number}";
            await Task.Delay(typeDelay);
        }
        int time = MauiProgram.businessLogic.artificialIntelligenceIllness.ContagiousDays;
        ContagiousDaysNumber.Text = "1";
        for(int timeInDays = 1; timeInDays < time; timeInDays++)
        {
            ContagiousDaysNumber.Text = $"{double.Parse(ContagiousDaysNumber.Text) + 1}";
            await Task.Delay(typeDelay);
        }
        return this.IsLoaded;
    }

    /// <summary>
    /// A function that draws the recovery period section of the page and
    /// lets the page know if it should continue drawing after
    /// </summary>
    private async Task<bool> DrawRecoveryPeriod()
    {
        RecoveryPeriod.IsVisible = true;
        foreach(char letter in recoveryPeriod.ToCharArray()) // printing header
        {
            RecoveryHeader.Text = $"{RecoveryHeader.Text}{letter}";
            await Task.Delay(typeDelay);
        }
        foreach(char letter in average.ToCharArray()) // printing average message
        {
            RecoveryAverage.Text = $"{RecoveryAverage.Text}{letter}";
            await Task.Delay(typeDelay);
        }
        foreach(char letter in hours.ToCharArray()) // printing recovery hours text
        {
            RecoveryHoursText.Text = $"{RecoveryHoursText.Text}{letter}";
            await Task.Delay(typeDelay);
        }
        int numHours = MauiProgram.businessLogic.artificialIntelligenceIllness!.RecoveryPeriod;
        RecoveryHoursNumber.Text = "1";
        for(int value = 1; value < numHours; value++) // printing recovery hours
        {
            RecoveryHoursNumber.Text = $"{int.Parse(RecoveryHoursNumber.Text) + 1}";
            await Task.Delay(intTypeDelay);
        }
        foreach(char letter in days.ToCharArray()) // printing recovery days text
        {
            RecoveryDaysText.Text = $"{RecoveryDaysText.Text}{letter}";
            await Task.Delay(typeDelay);
        }
        foreach(char number in RecoveryHoursNumber.Text.ToCharArray()) // printing recovery days number
        {
            RecoveryDaysNumber.Text = $"{RecoveryDaysNumber.Text}{number}";
            await Task.Delay(typeDelay);
        }
        foreach(char letter in division.ToCharArray())
        {
            RecoveryDaysNumber.Text = $"{RecoveryDaysNumber.Text}{letter}";
            await Task.Delay(typeDelay);
        }
        foreach(char number in "24".ToCharArray())
        {
            RecoveryDaysNumber.Text = $"{RecoveryDaysNumber.Text}{number}";
            await Task.Delay(typeDelay);
        }
        int time = MauiProgram.businessLogic.artificialIntelligenceIllness.RecoveryDays;
        RecoveryDaysNumber.Text = "1";
        for(int timeInDays = 1; timeInDays < time; timeInDays++)
        {
            RecoveryDaysNumber.Text = $"{double.Parse(RecoveryDaysNumber.Text) + 1}";
            await Task.Delay(typeDelay);
        }
        return this.IsLoaded;
    }

    /// <summary>
    /// A function that draws the symptom list section of the page
    /// </summary>
    private async Task<bool> DrawSymptomList()
    {
        Symptoms.IsVisible = true;
        foreach(char letter in fullSymptomList.ToCharArray()) // printing header
        {
            SymptomHeader.Text = $"{SymptomHeader.Text}{letter}";
            await Task.Delay(typeDelay);
        }
        foreach(char letter in cdc.ToCharArray()) // printing disclaimer message
        {
            Disclaimer.Text = $"{Disclaimer.Text}{letter}";
            await Task.Delay(typeDelay);
        }
        string[] symptoms = MauiProgram.businessLogic.artificialIntelligenceIllness!.Symptoms!;
        int rowsNeeded = (int)Math.Ceiling(symptoms.Length / 2.0);
        for (int row = 0; row < rowsNeeded; row++) // building all the rows needed
        {
            SymptomGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }
        int index = 0;
        for (int row = 0; row < rowsNeeded; row++) // adding each symptom
        {
            for (int column = 0; column < 2; column++)
            {
                if (index < symptoms.Length)
                {
                    Label label = new Label();
                    label.FontAttributes = FontAttributes.Bold;
                    label.HorizontalTextAlignment = TextAlignment.Center;
                    label.VerticalTextAlignment = TextAlignment.Center; // providing appealing vertical spacing when a symptom expands several rows
                    SymptomGrid.Add(label, column, row);
                    foreach (char letter in symptoms[index])
                    {
                        label.Text = $"{label.Text}{letter}";
                        await Task.Delay(typeDelay);
                    }
                    index++;
                }
            }
        }
        return true;
    }
}