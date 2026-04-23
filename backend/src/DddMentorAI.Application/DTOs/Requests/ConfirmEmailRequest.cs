using System.ComponentModel.DataAnnotations;

namespace DddMentorAI.Application.DTOs.Requests;

/// <summary>
/// Request model for email confirmation.
/// </summary>
public class ConfirmEmailRequest
{
    [Required(ErrorMessage = "User ID is required")]
    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Token is required")]
    public string Token { get; set; } = string.Empty;
}