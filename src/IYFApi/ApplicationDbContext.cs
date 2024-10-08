﻿using IYFApi.Exceptions;
using IYFApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IYFApi;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Post> Posts { get; set; } = null!;

    public virtual DbSet<Tag> Tags { get; set; } = null!;

    public virtual DbSet<PostsTag> PostsTags { get; set; } = null!;

    public virtual DbSet<Event> Events { get; set; } = null!;

    public virtual DbSet<EventGuest> EventGuests { get; set; } = null!;
    
    public virtual DbSet<EventCustomField> EventCustomFields { get; set; } = null!;

    public virtual DbSet<RegularEvent> RegularEvents { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql(
            GetConnectionString() + "Convert Zero Datetime=true;",
            ServerVersion.Parse("8.0.35-mysql"));
    
    private static string GetConnectionString() =>
        Environment.GetEnvironmentVariable("IYF_DB_CONNECTION_STRING")
        ?? throw new ConfigurationException("Cannot find connection string!");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Status)
                .HasConversion<StatusConverter>()
                .HasDefaultValueSql("'draft'");

            entity.Property(e => e.CreatedAt)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Tag>(entity => { entity.HasKey(e => e.Id).HasName("PRIMARY"); });

        modelBuilder.Entity<PostsTag>(entity =>
        {
            entity.HasKey(e => new { e.PostId, e.TagId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Status)
                .HasConversion<StatusConverter>()
                .HasDefaultValueSql("'draft'");

            entity.Property(e => e.CreatedAt)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<EventGuest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.CreatedAt)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
        
        modelBuilder.Entity<EventCustomField>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<RegularEvent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Status)
                .HasConversion<StatusConverter>()
                .HasDefaultValueSql("'draft'");

            entity.Property(e => e.CreatedAt)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }

    private class StatusConverter() : ValueConverter<Status, string>(v => v.ToString().ToLowerInvariant(),
        v => (Status)Enum.Parse(typeof(Status), v, true));
}