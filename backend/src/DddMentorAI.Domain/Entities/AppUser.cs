namespace DddMentorAI.Domain.Entities;

/// <summary>
/// Base entity for the application.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}