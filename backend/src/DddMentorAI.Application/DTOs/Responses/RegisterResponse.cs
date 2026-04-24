namespace DddMentorAI.Application.DTOs.Responses;

/// <summary>
/// Response model for successful user registration.
/// </summary>
public class RegisterResponse
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}