using DddMentorAI.Domain.Entities;
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

    // Study Sessions
    public DbSet<StudySession> StudySessions { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Customize the AppUser entity if needed
        builder.Entity<AppUser>(entity =>
        {
            entity.Property(u => u.Name).HasMaxLength(100);
        });

        // StudySession configuration
        builder.Entity<StudySession>(entity =>
        {
            entity.ToTable("StudySessions");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Title).HasMaxLength(200).IsRequired();
            entity.Property(s => s.Topic).HasMaxLength(500).IsRequired();
            entity.Property(s => s.Level).HasConversion<int>();
            entity.Property(s => s.UserId).HasMaxLength(450).IsRequired();
            entity.HasIndex(s => s.UserId);
            entity.HasMany(s => s.Messages)
                  .WithOne(m => m.StudySession)
                  .HasForeignKey(m => m.StudySessionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Message configuration
        builder.Entity<Message>(entity =>
        {
            entity.ToTable("Messages");
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Content).IsRequired();
            entity.Property(m => m.Role).HasConversion<int>();
            entity.Property(m => m.StudySessionId).IsRequired();
            entity.HasIndex(m => m.StudySessionId);
            entity.HasIndex(m => m.CreatedAt);
        });
    }
}