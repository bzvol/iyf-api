using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IYFApi.Models;

[PrimaryKey("PostId", "TagId")]
[Table("posts_tags")]
public class PostsTag
{
    [Key]
    [Column("post_id")]
    public ulong PostId { get; init; }
    public Post Post { get; init; } = null!;

    [Key]
    [Column("tag_id")]
    public ulong TagId { get; init; }
    public Tag Tag { get; init; } = null!;
}
