namespace Mapidemic.Models;

/// <summary>
/// Represents application settings with theme and postal code.
/// </summary>
public class Settings
{
    public AppTheme Theme { get; set; }
    public int PostalCode { get; set; }

    public Settings(AppTheme theme, int postalCode)
    {
        Theme = theme;
        PostalCode = postalCode;
    }
}