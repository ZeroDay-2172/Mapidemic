using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace Mapidemic.Models
{
    [Table("illnesses")]
    public class Illness : BaseModel
    {
        [PrimaryKey("illness", false)]
        public string? Name { get; set; }

        [Column("symptoms")]
        public string[]? Symptoms { get; set; }

        [Column("contagious_period")]
        public int ContagiousPeriod { get; set; }

        [Column("recovery_period")]
        public int RecoveryPeriod { get; set; }
    }
}