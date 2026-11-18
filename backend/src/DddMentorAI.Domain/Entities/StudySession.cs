using DddMentorAI.Domain.Common;
using DddMentorAI.Domain.Enums;
using DddMentorAI.Domain.ValueObjects;

namespace DddMentorAI.Domain.Entities;

/// <summary>
/// Agregado raiz: uma sessão de estudo de DDD de um usuário.
/// Toda alteração na sessão ou nas suas mensagens passa por aqui.
/// </summary>
public class StudySession
{
    private readonly List<Message> _messages = new();

    // Construtor exigido pelo EF Core.
    private StudySession()
    {
    }

    private StudySession(string userId, SessionTitle title, StudyTopic topic, StudyLevel level, DateTime utcNow)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Title = title;
        Topic = topic;
        Level = level;
        CreatedAt = utcNow;
        UpdatedAt = utcNow;
    }

    public Guid Id { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public SessionTitle Title { get; private set; } = null!;
    public StudyTopic Topic { get; private set; } = null!;
    public StudyLevel Level { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();

    public static StudySession Start(string userId, SessionTitle title, StudyTopic topic, StudyLevel level, DateTime utcNow)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new DomainException("A study session must belong to a user");
        }

        if (!Enum.IsDefined(level))
        {
            throw new DomainException("Invalid study level");
        }

        ArgumentNullException.ThrowIfNull(title);
        ArgumentNullException.ThrowIfNull(topic);

        return new StudySession(userId, title, topic, level, Guard.Utc(utcNow, "Creation time"));
    }

    public bool IsOwnedBy(string userId) => UserId == userId;

    /// <summary>
    /// Registra uma pergunta/mensagem do usuário e marca a sessão como atualizada.
    /// </summary>
    public Message AddUserMessage(MessageContent content, DateTime utcNow)
    {
        ArgumentNullException.ThrowIfNull(content);
        Guard.Utc(utcNow, "Message time");

        var message = new Message(Id, MessageRole.User, content, utcNow);
        _messages.Add(message);
        UpdatedAt = utcNow;

        return message;
    }
}
