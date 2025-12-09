using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
namespace Mapidemic.Models;

[Table("zip_illness_counts")]

/// <summary>
/// A class that represents illness counts for a specific zip code
/// </summary>
public class ZipIllnessCounts : BaseModel
{
    [PrimaryKey("postal_code")]
    public int PostalCode { get; set; }

    [Column("illness_type")]
    public string? IllnessType { get; set; }

    [Column("total_count")]
    public int TotalCount { get; set; }

    [Column("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }
}