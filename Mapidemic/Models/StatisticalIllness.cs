namespace Mapidemic.Models;

/// <summary>
/// A class that represents an illness along with its associated probability
/// </summary>
public class StatisticalIllness
{
    public Illness Illness { get; set; }
    public double Probability { get; set; }

    /// <summary>
    /// The default constructor for a StatisticalIllness
    /// </summary>
    /// <param name="illness">The illness being represented</param>
    /// <param name="probability">The probability of the illness</param>
    public StatisticalIllness(Illness illness, double probability)
    {
        Illness = illness;
        Probability = probability;
    }
}