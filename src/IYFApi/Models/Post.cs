using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IYFApi.Models;

[Table("posts")]
[Index("Id", Name = "id", IsUnique = true)]
public class Post
{
    [Key]
    [Column("id")]
    public ulong Id { get; init; }

    [Column("title")]
    [StringLength(255)]
    public string Title { get; set; } = null!;

    [Column("content", TypeName = "text")]
    public string Content { get; set; } = null!;

    [Column("status", TypeName = "enum('draft','published','archived')")]
    public Status Status { get; set; } = Status.Draft;

    [Column("created_at", TypeName = "timestamp")]
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    
    [Column("created_by")]
    [StringLength(255)]
    public string CreatedBy { get; init; } = null!;

    [Column("updated_at", TypeName = "timestamp")]
    public DateTime UpdatedAt { get; init; } = DateTime.Now;
    
    [Column("updated_by")]
    [StringLength(255)]
    public string UpdatedBy { get; set; } = null!;
}
