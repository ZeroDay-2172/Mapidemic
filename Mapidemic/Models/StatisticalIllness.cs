namespace Mapidemic.Models;

public class StatisticalIllness
{
    public Illness Illness { get; set; }
    public double Probability { get; set; }

    public StatisticalIllness(Illness illness, double probability)
    {
        Illness = illness;
        Probability = probability;
    }
}