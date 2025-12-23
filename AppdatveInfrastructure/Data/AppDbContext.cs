using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppdatveCore.Entities;

namespace AppdatveInfrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<BusCompany> BusCompanies { get; set; }
    public DbSet<Bus> Busess { get; set; }
    public DbSet<Trip> Trips { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
    base.OnModelCreating(builder);

    // Cấu hình độ chính xác cho tiền tệ (18 chữ số, 2 số lẻ)
    builder.Entity<Trip>()
        .Property(t => t.Price)
        .HasPrecision(18, 2);

    builder.Entity<Booking>()
        .Property(b => b.TotalAmount)
        .HasPrecision(18, 2);
    }
}
