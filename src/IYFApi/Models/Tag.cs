using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IYFApi.Models;

[Table("tags")]
[Index("Id", Name = "id")]
[Index("Name", Name = "name")]
public class Tag
{
    [Key]
    [Column("id")]
    public ulong Id { get; init; }

    [Column("name")]
    public string Name { get; init; } = null!;
}
