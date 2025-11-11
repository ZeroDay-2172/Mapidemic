using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
namespace Mapidemic.Models;

[Table("illness_reports")]
public class IllnessReport : BaseModel
{
    [PrimaryKey("uuid", false)]
    public Guid Id { get; set; }

    [Column("postal_code")]
    public int PostalCode { get; set; }

    [Column("illness_type")]
    public string? IllnessType { get; set; }

    [Column("report_date")]
    public DateTimeOffset ReportDate { get; set; }
}