using DddMentorAI.Domain.Enums;

namespace DddMentorAI.Domain.Entities;

/// <summary>
/// Represents a study session in the DDD learning platform.
/// </summary>
public class StudySession
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public StudyLevel Level { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}