using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Mapidemic.Models;

[Table("feedback")]
public class Feedback : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("category")]
    public string? Category { get; set; }

    [Column("message")]
    public string Message { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

}