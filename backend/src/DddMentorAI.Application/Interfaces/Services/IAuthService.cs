using DddMentorAI.Application.DTOs.Requests;
using DddMentorAI.Application.DTOs.Responses;

namespace DddMentorAI.Application.Interfaces.Services;

/// <summary>
/// Interface for authentication operations.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);

    /// <summary>
    /// Confirms a user's email address.
    /// </summary>
    Task<ApiResponse<string>> ConfirmEmailAsync(ConfirmEmailRequest request);

    /// <summary>
    /// Resends the email confirmation link to a user.
    /// </summary>
    Task<ApiResponse<string>> ResendConfirmationAsync(ResendConfirmationRequest request);
}