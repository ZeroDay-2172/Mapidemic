using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
namespace Mapidemic.Models;

[Table("population")]

public class Population : BaseModel
{
    [PrimaryKey("name", false)]
    public string? Name { get; set; }

    [Column("postal_code") ]
    public int PostalCode { get; set; }

    [Column("population_count")]
    public int PopulationCount { get; set; }
}