using DddMentorAI.Domain.Enums;

namespace DddMentorAI.Domain.Entities;

/// <summary>
/// Entidade que representa uma sessão de estudo sobre DDD.
/// </summary>
public class StudySession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public StudyLevel Level { get; set; } = StudyLevel.Beginner;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}