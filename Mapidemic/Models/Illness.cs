using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace Mapidemic.Models
{
    [Table("illnesses")]
    public class Illness : BaseModel
    {
        private const double HoursInDay = 24.0;

        [PrimaryKey("illness", false)]
        public string? Name { get; set; }

        [Column("symptoms")]
        public string[]? Symptoms { get; set; }

        [Column("contagious_period_hours")]
        public int ContagiousPeriod { get; set; }

        [Column("recovery_period_hours")]
        public int RecoveryPeriod { get; set; }

        public int ContagiousDays
        {
            get => (int)Math.Ceiling(ContagiousPeriod / HoursInDay);
        }

        public int RecoveryDays
        {
            get => (int)Math.Ceiling(RecoveryPeriod / HoursInDay);
        }
    }
}