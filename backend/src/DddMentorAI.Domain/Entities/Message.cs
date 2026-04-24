using DddMentorAI.Domain.Enums;

namespace DddMentorAI.Domain.Entities;

/// <summary>
/// Entidade que representa uma mensagem dentro de uma sessão de estudo.
/// </summary>
public class Message
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid StudySessionId { get; set; }
    public MessageRole Role { get; set; } = MessageRole.User;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual StudySession? StudySession { get; set; }
}