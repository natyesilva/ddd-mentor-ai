using DddMentorAI.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DddMentorAI.Infrastructure.Data;

/// <summary>
/// Application database context integrating ASP.NET Core Identity with Entity Framework Core.
/// </summary>
public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Customize the AppUser entity if needed
        builder.Entity<AppUser>(entity =>
        {
            entity.Property(u => u.Name).HasMaxLength(100);
        });
    }
}