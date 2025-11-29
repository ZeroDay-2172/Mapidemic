namespace Mapidemic.Pages.SymptomChecker;

public partial class AiResultsPage : ContentPage
{
    private const int typeDelay = 25;
    private readonly string greeting = "Hello, my name is Walter!";
    private readonly string firstLine = "After reviewing your symptoms, I think they align most closely with ";
    private readonly string secondLine = "Let me tell you a little about this illness. This illness has a contagious period of ";
    private readonly string altSecondLine = "Let me tell you a little about this illness. The contagious period is not typically the same and can vary.";
    private readonly string thirdLine = "You won't have to worry too long if you get sick with this illness. This illness has a recovery period of ";
    private readonly string altThirdLine = "After searching the web for a recovery period, I could not locate one. It appears that the recovery period varies.";
    private readonly string fourthLine = "I'll show you the symptom list for this illness that is provided by the Center for Disease Control and prevention.";
    private readonly string altFourthLine = "The symptoms you provided for me may not have been conclusive for this illness. Unfortunately, I was not able to locate a full list of symptoms for you. It is possible more symptoms are associated with this illness.";

    public AiResultsPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        await Task.Delay(500);

        foreach(char letter in greeting.ToCharArray())
        {
            Greeting.Text = $"{Greeting.Text}{letter}";
            await Task.Delay(typeDelay);
        }

        foreach(char letter in firstLine.ToCharArray())
        {
            FirstRobotText.Text = $"{FirstRobotText.Text}{letter}";
            await Task.Delay(typeDelay);
        }

        string illness = MauiProgram.businessLogic.artificialIntelligenceIllness!.Name!;
        if (illness == "Flu")
        {
            illness = "Influenza";
        }

        foreach(char letter in illness.ToCharArray())
        {
            IllnessText.Text = $"{IllnessText.Text}{letter}";
            await Task.Delay(typeDelay);
        }
        IllnessText.Text = $"{IllnessText.Text}.";

        

        int contagiousPeriod = MauiProgram.businessLogic.artificialIntelligenceIllness.ContagiousPeriod;
        if (contagiousPeriod == -1)
        {
            foreach(char letter in altSecondLine.ToCharArray())
            {
                SecondRobotText.Text = $"{SecondRobotText.Text}{letter}";
                await Task.Delay(typeDelay);
            }
        }
        else
        {
            foreach(char letter in secondLine.ToCharArray())
            {
                SecondRobotText.Text = $"{SecondRobotText.Text}{letter}";
                await Task.Delay(typeDelay);
            }

            foreach(char letter in contagiousPeriod.ToString().ToCharArray())
            {
                ContagiousText.Text = $"{ContagiousText.Text}{letter}";
                await Task.Delay(typeDelay);
            }

            foreach(char letter in " hour(s) / ".ToCharArray())
            {
                ContagiousText.Text = $"{ContagiousText.Text}{letter}";
                await Task.Delay(typeDelay);
            }

            foreach(char letter in MauiProgram.businessLogic.artificialIntelligenceIllness.ContagiousDays.ToString().ToCharArray())
            {
                ContagiousText.Text = $"{ContagiousText.Text}{letter}";
                await Task.Delay(typeDelay);
            }

            foreach(char letter in " day(s).".ToCharArray())
            {
                ContagiousText.Text = $"{ContagiousText.Text}{letter}";
                await Task.Delay(typeDelay);
            }
        }

        int recoveryPeriod = MauiProgram.businessLogic.artificialIntelligenceIllness.RecoveryPeriod;
        if (recoveryPeriod == -1)
        {
            foreach(char letter in altThirdLine.ToCharArray())
            {
                ThirdRobotText.Text = $"{ThirdRobotText.Text}{letter}";
                await Task.Delay(typeDelay);
            }
        }
        else
        {
            foreach(char letter in thirdLine.ToCharArray())
            {
                ThirdRobotText.Text = $"{ThirdRobotText.Text}{letter}";
                await Task.Delay(typeDelay);
            }

            foreach(char letter in recoveryPeriod.ToString().ToCharArray())
            {
                RecoveryText.Text = $"{RecoveryText.Text}{letter}";
                await Task.Delay(typeDelay);
            }

            foreach(char letter in " hour(s) / ".ToCharArray())
            {
                RecoveryText.Text = $"{RecoveryText.Text}{letter}";
                await Task.Delay(typeDelay);
            }

            foreach(char letter in MauiProgram.businessLogic.artificialIntelligenceIllness.RecoveryDays.ToString().ToCharArray())
            {
                RecoveryText.Text = $"{RecoveryText.Text}{letter}";
                await Task.Delay(typeDelay);
            }

            foreach(char letter in " day(s).".ToCharArray())
            {
                RecoveryText.Text = $"{RecoveryText.Text}{letter}";
                await Task.Delay(typeDelay);
            }
        }

        string[] symptoms = MauiProgram.businessLogic.artificialIntelligenceIllness.Symptoms!;
        if (symptoms.Length == 0)
        {
            foreach(char letter in altFourthLine.ToCharArray())
            {
                FourthRobotText.Text = $"{FourthRobotText.Text}{letter}";
                await Task.Delay(typeDelay);
            }
        }
        else
        {
            foreach(char letter in fourthLine.ToCharArray())
            {
                FourthRobotText.Text = $"{FourthRobotText.Text}{letter}";
                await Task.Delay(typeDelay);
            }

            int rowsNeeded = (int)Math.Ceiling(symptoms.Length / 2.0);
            for (int row = 0; row < rowsNeeded; row++)
            {
                SymptomGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            int index = 0;
            for (int row = 0; row < rowsNeeded; row++)
            {
                for (int column = 0; column < 2; column++)
                {
                    if (index < symptoms.Length)
                    {
                        Label label = new Label();
                        label.FontAttributes = FontAttributes.Bold;
                        label.HorizontalTextAlignment = TextAlignment.Center;
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
        }
    }
}