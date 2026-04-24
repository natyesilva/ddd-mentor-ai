using System.ComponentModel.DataAnnotations;

namespace DddMentorAI.Application.DTOs.Requests;

/// <summary>
/// Request model for user login.
/// </summary>
public class LoginRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}