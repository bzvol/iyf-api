﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IYFApi.Models;

[Table("regular_events")]
[Index("Id", Name = "id", IsUnique = true)]
public class RegularEvent
{
    [Key]
    [Column("id")]
    public ulong Id { get; init; }

    [Column("title")]
    [StringLength(255)]
    public string Title { get; set; } = null!;

    [Column("details", TypeName = "text")]
    public string Details { get; set; } = null!;
    
    [Column("time")]
    [StringLength(255)]
    public string? Time { get; set; } = null!;
    
    [Column("location")]
    [StringLength(255)]
    public string? Location { get; set; } = null!;

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
