using DddMentorAI.Domain.Enums;

namespace DddMentorAI.Application.DTOs.Responses;

/// <summary>
/// Response model for a study session.
/// </summary>
public class StudySessionResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public StudyLevel Level { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int MessageCount { get; set; }
}