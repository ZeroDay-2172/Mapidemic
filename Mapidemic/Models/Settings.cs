namespace Mapidemic.Models;

/// <summary>
/// Represents application settings with theme and postal code.
/// </summary>
public class Settings
{
    public AppTheme Theme { get; set; }
    public int PostalCode { get; set; }

    /// <summary>
    /// The designated constructor for a Settings object
    /// </summary>
    /// <param name="theme"></param>
    /// <param name="postalCode"></param>
    public Settings(AppTheme theme, int postalCode)
    {
        Theme = theme;
        PostalCode = postalCode;
    }
}