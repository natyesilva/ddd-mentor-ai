using DddMentorAI.Domain.Enums;

namespace DddMentorAI.Application.DTOs.Responses.StudySession;

/// <summary>
/// Response para detalhes de uma sessão de estudo.
/// </summary>
public class StudySessionDetailsResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public StudyLevel Level { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int MessageCount { get; set; }
}