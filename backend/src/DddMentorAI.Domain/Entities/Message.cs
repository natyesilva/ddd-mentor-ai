using DddMentorAI.Domain.Enums;
using DddMentorAI.Domain.ValueObjects;

namespace DddMentorAI.Domain.Entities;

/// <summary>
/// Mensagem de uma sessão de estudo. Só é criada pelo agregado <see cref="StudySession"/>.
/// </summary>
public class Message
{
    // Construtor exigido pelo EF Core.
    private Message()
    {
    }

    internal Message(Guid studySessionId, MessageRole role, MessageContent content, DateTime createdAt)
    {
        Id = Guid.NewGuid();
        StudySessionId = studySessionId;
        Role = role;
        Content = content;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public Guid StudySessionId { get; private set; }
    public MessageRole Role { get; private set; }
    public MessageContent Content { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
}
