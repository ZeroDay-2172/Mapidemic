using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace Mapidemic.Models
{
    [Table("symptoms")]
    public class Symptom : BaseModel
    {
        [PrimaryKey("symptom", false)]
        public string? Name { get; set; }
    }
}