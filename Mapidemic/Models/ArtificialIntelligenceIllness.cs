namespace Mapidemic.Models;

public class ArtificialIntelligenceIllness
{
    private const double HoursInDay = 24.0;

    public string? Name { get; set; }
    public string[]? Symptoms { get; set; }
    public int ContagiousPeriod { get; set; }
    public int RecoveryPeriod { get; set; }
    public int ContagiousDays
    {
        get => (int)Math.Ceiling(ContagiousPeriod / HoursInDay);
    }
    public int RecoveryDays
    {
        get => (int)Math.Ceiling(RecoveryPeriod / HoursInDay);
    }

    /// <summary>
    /// The designated constructor for an ArtificialIntelligenceIllness
    /// </summary>
    /// <param name="name"></param>
    /// <param name="symptoms"></param>
    /// <param name="contagiousPeriod"></param>
    /// <param name="recoveryPeriod"></param>
    public ArtificialIntelligenceIllness(string name, string[] symptoms, int contagiousPeriod, int recoveryPeriod)
    {
        Name = name;
        Symptoms = symptoms;
        ContagiousPeriod = contagiousPeriod;
        RecoveryPeriod = recoveryPeriod;
    }
}