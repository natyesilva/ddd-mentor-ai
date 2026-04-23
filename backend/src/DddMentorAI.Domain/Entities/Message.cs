using DddMentorAI.Domain.Enums;

namespace DddMentorAI.Domain.Entities;

/// <summary>
/// Represents a message within a study session.
/// </summary>
public class Message
{
    public Guid Id { get; set; }
    public Guid StudySessionId { get; set; }
    public MessageRole Role { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public virtual StudySession? StudySession { get; set; }
}