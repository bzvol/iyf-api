using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IYFApi.Models;

[Table("tags")]
[Index("Id", Name = "id", IsUnique = true)]
[Index("Name", Name = "name", IsUnique = true)]
public partial class Tag
{
    [Key]
    [Column("id")]
    public ulong Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;
    
    // public ICollection<PostsTag> PostsTags { get; set; } = new List<PostsTag>();
}
