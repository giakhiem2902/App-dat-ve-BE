using Microsoft.AspNetCore.Identity;

namespace AppdatveCore.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}