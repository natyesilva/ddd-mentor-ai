using DddMentorAI.Application.Interfaces.Persistence;
using DddMentorAI.Domain.Entities;
using DddMentorAI.Domain.ValueObjects;
using DddMentorAI.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DddMentorAI.Infrastructure.Data;

/// <summary>
/// Application database context integrating ASP.NET Core Identity with Entity Framework Core.
/// </summary>
public class AppDbContext : IdentityDbContext<AppUser>, IUnitOfWork
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
            // Ids são gerados pelo domínio; sem isso o EF trataria mensagens novas do agregado como UPDATE.
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Title)
                .HasConversion(v => v.Value, v => SessionTitle.From(v))
                .HasMaxLength(SessionTitle.MaxLength).IsRequired();
            entity.Property(e => e.Topic)
                .HasConversion(v => v.Value, v => StudyTopic.From(v))
                .HasMaxLength(StudyTopic.MaxLength).IsRequired();
            entity.Property(e => e.UserId).HasMaxLength(450).IsRequired();
            entity.Property(e => e.Level).HasConversion<int>();

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.UpdatedAt);

            entity.HasMany(e => e.Messages)
                .WithOne()
                .HasForeignKey(m => m.StudySessionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.Navigation(e => e.Messages)
                .HasField("_messages")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        // Message configuration
        builder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Content)
                .HasConversion(v => v.Value, v => MessageContent.From(v))
                .IsRequired();
            entity.Property(e => e.Role).HasConversion<int>();
            entity.Property(e => e.CreatedAt).HasColumnType("timestamp with time zone");

            entity.HasIndex(e => e.StudySessionId);
            entity.HasIndex(e => e.CreatedAt);
        });
    }
}