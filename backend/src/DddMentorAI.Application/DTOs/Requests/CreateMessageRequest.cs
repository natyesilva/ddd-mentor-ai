using System.ComponentModel.DataAnnotations;

namespace DddMentorAI.Application.DTOs.Requests;

/// <summary>
/// Request model for creating a new message in a study session.
/// </summary>
public class CreateMessageRequest
{
    [Required(ErrorMessage = "Content is required")]
    [StringLength(4000, MinimumLength = 1, ErrorMessage = "Content must be between 1 and 4000 characters")]
    public string Content { get; set; } = string.Empty;
}