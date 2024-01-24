﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IYFApi.Models;

[Table("event_visitors")]
[Index("Id", Name = "id", IsUnique = true)]
public partial class EventVisitor
{
    [Key]
    [Column("id")]
    public ulong Id { get; set; }

    [Column("event_id")]
    public int EventId { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string? Name { get; set; }

    [Column("email")]
    [StringLength(255)]
    public string? Email { get; set; }

    [Column("phone")]
    [StringLength(30)]
    public string? Phone { get; set; }

    [Column("city")]
    [StringLength(30)]
    public string? City { get; set; }

    [Column("source")]
    [StringLength(255)]
    public string? Source { get; set; }

    [Column("added_at", TypeName = "timestamp")]
    public DateTime AddedAt { get; set; }
}
