namespace DddMentorAI.Application.DTOs.Responses;

/// <summary>
/// Response model for successful login.
/// </summary>
public class LoginResponse
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}