namespace Mapidemic.Models;

public class AnalyzedIllness
{
    public Illness Illness { get; set; }
    public double Probability { get; set; }

    public AnalyzedIllness(Illness illness, double probability)
    {
        Illness = illness;
        Probability = probability;
    }
}