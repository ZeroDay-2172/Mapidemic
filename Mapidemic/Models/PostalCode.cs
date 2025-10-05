using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace Mapidemic.Models
{
    [Table("postal_codes")]
    public class PostalCode : BaseModel
    {
        [PrimaryKey("postal_code", false)]
        public int Code { get; set; }
    }  
}