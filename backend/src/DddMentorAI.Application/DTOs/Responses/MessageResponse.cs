using DddMentorAI.Domain.Enums;

namespace DddMentorAI.Application.DTOs.Responses;

/// <summary>
/// Response model for a message.
/// </summary>
public class MessageResponse
{
    public Guid Id { get; set; }
    public Guid StudySessionId { get; set; }
    public MessageRole Role { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}