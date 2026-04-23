using System.ComponentModel.DataAnnotations;

namespace DddMentorAI.Application.DTOs.Requests;

/// <summary>
/// Request model for creating a new study session.
/// </summary>
public class CreateStudySessionRequest
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Topic is required")]
    [StringLength(500, MinimumLength = 2, ErrorMessage = "Topic must be between 2 and 500 characters")]
    public string Topic { get; set; } = string.Empty;

    [Required(ErrorMessage = "Level is required")]
    public int Level { get; set; }
}