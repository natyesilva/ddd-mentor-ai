using System.ComponentModel.DataAnnotations;

namespace DddMentorAI.Application.DTOs.Requests.StudySession;

/// <summary>
/// Request para criar uma nova mensagem.
/// </summary>
public class CreateMessageRequest
{
    [Required(ErrorMessage = "Content is required")]
    [StringLength(10000, MinimumLength = 1, ErrorMessage = "Content must be between 1 and 10000 characters")]
    public string Content { get; set; } = string.Empty;
}