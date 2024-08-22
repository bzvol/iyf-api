using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IYFApi.Models;

[Table("events")]
[Index("Id", Name = "id", IsUnique = true)]
public class Event
{
    [Key]
    [Column("id")]
    public ulong Id { get; init; }

    [Column("title")]
    [StringLength(255)]
    public string Title { get; set; } = null!;

    [Column("details", TypeName = "text")]
    public string Details { get; set; } = null!;
    
    [Column("time_start")]
    public DateTime? StartTime { get; set; }
    
    [Column("time_end")]
    public DateTime? EndTime { get; set; }
    
    [Column("location")]
    [StringLength(255)]
    public string? Location { get; set; }

    [Column("status", TypeName = "enum('draft','published','archived')")]
    public Status Status { get; set; }

    [Column("created_at", TypeName = "timestamp")]
    public DateTime CreatedAt { get; init; }
    
    [Column("created_by")]
    [StringLength(255)]
    public string CreatedBy { get; init; } = null!;

    [Column("updated_at", TypeName = "timestamp")]
    public DateTime UpdatedAt { get; init; }
    
    [Column("updated_by")]
    [StringLength(255)]
    public string UpdatedBy { get; set; } = null!;
    
    [Column("published_at", TypeName = "timestamp")]
    public DateTime? PublishedAt { get; set; }
}
