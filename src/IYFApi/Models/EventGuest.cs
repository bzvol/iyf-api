using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IYFApi.Models;

[Table("event_guests")]
[Index("Id", Name = "id", IsUnique = true)]
[Index("EventId", Name = "event_id")]
public class EventGuest
{
    [Key]
    [Column("id")]
    public ulong Id { get; init; }

    [Column("event_id")]
    public ulong EventId { get; init; }

    [Column("name")]
    [StringLength(255)]
    public string? Name { get; set; }

    [Column("email")]
    [StringLength(255)]
    public string? Email { get; set; }

    [Column("phone")]
    [StringLength(30)]
    public string? Phone { get; set; }
    
    [Column("age")]
    public int? Age { get; set; }

    [Column("city")]
    [StringLength(30)]
    public string? City { get; set; }

    [Column("source")]
    [StringLength(255)]
    public string? Source { get; set; }

    [Column("added_at", TypeName = "timestamp")]
    public DateTime AddedAt { get; init; }
}
