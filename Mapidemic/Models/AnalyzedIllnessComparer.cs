namespace Mapidemic.Models;

/// <summary>
/// A comparer that is used to start
/// AnalyzedIllnesses by probability
/// </summary>
public class AnalyzedIllnessComparer : IComparer<AnalyzedIllness>
{
    /// <summary>
    /// A function that compares two analyzed illnesses
    /// and determines which has the high probability
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>Negative value if x, 0 is equal probability, positive if y</returns>
    public int Compare(AnalyzedIllness? x, AnalyzedIllness? y)
    {
        return (int)(y!.Probability - x!.Probability);
    }
}