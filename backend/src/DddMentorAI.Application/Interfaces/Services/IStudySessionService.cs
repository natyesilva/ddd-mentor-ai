using DddMentorAI.Application.DTOs.Requests;
using DddMentorAI.Application.DTOs.Responses;

namespace DddMentorAI.Application.Interfaces.Services;

/// <summary>
/// Interface for study session operations.
/// </summary>
public interface IStudySessionService
{
    /// <summary>
    /// Creates a new study session for the authenticated user.
    /// </summary>
    Task<ApiResponse<StudySessionResponse>> CreateAsync(string userId, CreateStudySessionRequest request);

    /// <summary>
    /// Gets all study sessions for the authenticated user.
    /// </summary>
    Task<ApiResponse<List<StudySessionResponse>>> GetAllAsync(string userId);

    /// <summary>
    /// Gets a study session by ID for the authenticated user.
    /// </summary>
    Task<ApiResponse<StudySessionDetailsResponse>> GetByIdAsync(string userId, Guid sessionId);

    /// <summary>
    /// Gets all messages for a study session.
    /// </summary>
    Task<ApiResponse<List<MessageResponse>>> GetMessagesAsync(string userId, Guid sessionId);

    /// <summary>
    /// Creates a new message in a study session.
    /// </summary>
    Task<ApiResponse<MessageResponse>> CreateMessageAsync(string userId, Guid sessionId, CreateMessageRequest request);
}