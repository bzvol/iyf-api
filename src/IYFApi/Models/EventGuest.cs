using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IYFApi.Models;

[Table("event_guests")]
[Index("Id", Name = "id", IsUnique = true)]
[Index("EventId", Name = "event_id")]
public class EventGuest
{
    [Key]
    [Column("id")]
    public ulong Id { get; init; }

    [Column("event_id")]
    public ulong EventId { get; init; }

    [Column("name")]
    [StringLength(255)]
    public string? Name { get; set; }

    [Column("email")]
    [StringLength(255)]
    public string? Email { get; set; }

    [Column("phone")]
    [StringLength(30)]
    public string? Phone { get; set; }
    
    [Column("age")]
    public int? Age { get; set; }

    [Column("city")]
    [StringLength(30)]
    public string? City { get; set; }

    [Column("source")]
    [StringLength(255)]
    public string? Source { get; set; }

    [Column("created_at", TypeName = "timestamp")]
    public DateTime CreatedAt { get; init; }

    public class EquivalencyComparer : IEqualityComparer<EventGuest>
    {
        public bool Equals(EventGuest? x, EventGuest? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            
            if (x.Id == y.Id) return true;

            var nameEquals = x.Name != null && y.Name != null && x.Name == y.Name;
            var emailEquals = x.Email != null && y.Email != null && x.Email == y.Email;
            var phoneEquals = x.Phone != null && y.Phone != null && x.Phone == y.Phone;

            // If name&email / name&phone / email&phone are same, then x is equivalent to y.
            return (nameEquals && (emailEquals || phoneEquals)) || (emailEquals && phoneEquals);
        }

        public int GetHashCode(EventGuest obj)
        {
            return HashCode.Combine(obj.Name, obj.Email, obj.Phone);
        }
    }
}
