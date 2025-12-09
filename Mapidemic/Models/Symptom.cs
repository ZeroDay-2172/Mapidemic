namespace Mapidemic.Models;

/// <summary>
/// A class that represents a symptom with a name and a checked state
/// </summary>
public class Symptom
{
    public string? Name { get; set; }
    public bool IsChecked { get; set; }

    /// <summary>
    /// The default constructor for a Symptom
    /// </summary>
    /// <param name="name">The name of the symptom</param>
    public Symptom(string name)
    {
        Name = name;
        IsChecked = false;
    }
}