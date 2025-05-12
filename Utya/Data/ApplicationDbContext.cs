using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Utya.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<ShortLink> ShortLinks { get; set; }
    public DbSet<Click> Clicks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ShortLink
        // modelBuilder.Entity<ShortLink>()
        //     .HasIndex(s => s.ShortCode)
        //     .IsUnique()
        //     .HasFilter("IsActive = true");
        //
        // modelBuilder.Entity<ShortLink>()
        //     .HasIndex(s => s.CustomAlias)
        //     .IsUnique()
        //     .HasFilter("CustomAlias IS NOT NULL");
        //
        // // Click
        // modelBuilder.Entity<Click>()
        //     .HasIndex(c => new { c.ShortLinkId, c.Timestamp });

        base.OnModelCreating(modelBuilder);
    }
}