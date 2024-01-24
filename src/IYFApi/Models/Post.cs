﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IYFApi.Models.Types;
using Microsoft.EntityFrameworkCore;

namespace IYFApi.Models;

[Table("posts")]
[Index("Id", Name = "id", IsUnique = true)]
public partial class Post
{
    [Key]
    [Column("id")]
    public ulong Id { get; set; }

    [Column("title")]
    [StringLength(255)]
    public string Title { get; set; } = null!;

    [Column("body", TypeName = "text")]
    public string Body { get; set; } = null!;

    [Column("status", TypeName = "enum('draft','published','archived')")]
    public Status Status { get; set; } = Status.Draft;

    [Column("created_at", TypeName = "timestamp")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at", TypeName = "timestamp")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // public ICollection<PostsTag> PostsTags { get; set; } = new List<PostsTag>();
}
