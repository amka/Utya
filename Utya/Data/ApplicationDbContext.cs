using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Utya.Shared.Models;

namespace Utya.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<ShortLink> ShortLinks { get; set; }
    public DbSet<Click> Clicks { get; set; }
    public DbSet<UserPlan> UserPlans { get; set; }
    public DbSet<Plan> Plans { get; set; }

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSeeding((context, _) =>
            {
                context.Set<Plan>().Add(new Plan
                {
                    Id = 1,
                    Price = 0,
                    Name = "Бесплатный", MaxLinks = 250, MaxClicksPerMonth = 10_000, Expiration = Expiration.OneMonth
                });
                context.Set<Plan>().Add(new Plan
                {
                    Id = 2,
                    Price = 499,
                    Name = "Базовый", MaxLinks = -1, MaxClicksPerMonth = 100_000, Expiration = Expiration.Never,
                    AllowAdvancedAnalytics = false
                });
                context.Set<Plan>().Add(new Plan
                {
                    Id = 3,
                    Price = 499,
                    Name = "Премиум", MaxLinks = -1, MaxClicksPerMonth = -1, Expiration = Expiration.Never,
                    AllowAdvancedAnalytics = true
                });
                context.SaveChanges();
            });
    }
}