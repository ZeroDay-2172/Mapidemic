namespace Mapidemic.Models;

public class Symptom
{
    public string? Name { get; set; }
    public bool IsChecked { get; set; }

    public Symptom(string name)
    {
        Name = name;
        IsChecked = false;
    }
}