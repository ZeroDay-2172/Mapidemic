using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
namespace Mapidemic.Models;

[Table("population")]

/// <summary>
/// Represents population data with its properties.
/// </summary>
public class Population : BaseModel
{
    [PrimaryKey("name", false)]
    public string? Name { get; set; }

    [Column("postal_code") ]
    public int PostalCode { get; set; }

    [Column("population_count")]
    public int PopulationCount { get; set; }
}