using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IYFApi.Models;

[Table("event_custom_fields")]
[Index("Id", Name = "id", IsUnique = true)]
[Index("EventId", Name = "event_id")]
[Index("GuestId", Name = "guest_id")]
public class EventCustomField
{
    [Key]
    [Column("id")]
    public ulong Id { get; init; }
    
    [Column("event_id")]
    public ulong EventId { get; init; }
    
    [Column("guest_id")]
    public ulong GuestId { get; init; }
    
    [Column("field_name")]
    [StringLength(255)]
    public string FieldName { get; init; } = null!;
    
    [Column("field_value")]
    [StringLength(255)]
    public string FieldValue { get; set; } = null!;
}