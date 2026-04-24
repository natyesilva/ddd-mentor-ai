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

    // DbSets for new entities
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
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Topic).HasMaxLength(500).IsRequired();
            entity.Property(e => e.UserId).HasMaxLength(450).IsRequired();
            entity.Property(e => e.Level).HasConversion<int>();

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.UpdatedAt);
        });

        // Message configuration
        builder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.Role).HasConversion<int>();
            entity.Property(e => e.CreatedAt).HasColumnType("timestamp with time zone");

            entity.HasIndex(e => e.StudySessionId);
            entity.HasIndex(e => e.CreatedAt);

            entity.HasOne(e => e.StudySession)
                .WithMany(s => s.Messages)
                .HasForeignKey(e => e.StudySessionId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}