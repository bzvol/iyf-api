using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IYFApi.Models;

[PrimaryKey("PostId", "TagId")]
[Table("posts_tags")]
public partial class PostsTag
{
    [Key]
    [Column("post_id")]
    public ulong PostId { get; set; }
    public Post Post { get; set; } = null!;

    [Key]
    [Column("tag_id")]
    public ulong TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}
