using System.ComponentModel.DataAnnotations;

namespace DddMentorAI.Application.DTOs.Requests;

/// <summary>
/// Request model for resending email confirmation.
/// </summary>
public class ResendConfirmationRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;
}