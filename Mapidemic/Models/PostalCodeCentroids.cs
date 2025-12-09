using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace Mapidemic.Models
{
    [Table("postal_code_centroids")]

    /// <summary>
    /// Represents postal code centroids with their properties.
    /// </summary>
    public class PostalCodeCentroids : BaseModel
    {
        [PrimaryKey("postal_code", false)]
        public int Code { get; set; }

        [Column("lat")]
        public double Latitude { get; set; }

        [Column("lon")]
        public double Longitude { get; set; }
    }  
}