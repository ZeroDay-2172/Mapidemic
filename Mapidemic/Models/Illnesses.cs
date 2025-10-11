using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace Mapidemic.Models;

[Table("illnesses")]
public class Illnesses : BaseModel
{
    [PrimaryKey("illness", false)]
    public string? Illness { get; set; }

    [Column("symptoms")]
    public string[]? Symptoms { get; set; }

    [Column("contagious_period (hours)")]
    public int? ContagiousPeriod { get; set; }

    [Column("recovery_period (hours)")]
    public int? RecoveryPeriod { get; set; }
}