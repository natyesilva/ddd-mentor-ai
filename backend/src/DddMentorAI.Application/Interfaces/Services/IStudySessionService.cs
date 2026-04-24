using DddMentorAI.Application.DTOs.Requests.StudySession;
using DddMentorAI.Application.DTOs.Responses.StudySession;
using DddMentorAI.Application.DTOs.Responses;

namespace DddMentorAI.Application.Interfaces.Services;

/// <summary>
/// Interface para operações de sessão de estudo.
/// </summary>
public interface IStudySessionService
{
    /// <summary>
    /// Cria uma nova sessão de estudo para o usuário autenticado.
    /// </summary>
    Task<ApiResponse<StudySessionResponse>> CreateAsync(string userId, CreateStudySessionRequest request);

    /// <summary>
    /// Lista todas as sessões do usuário autenticado.
    /// </summary>
    Task<ApiResponse<List<StudySessionResponse>>> GetAllAsync(string userId);

    /// <summary>
    /// Obtém os detalhes de uma sessão específica do usuário autenticado.
    /// </summary>
    Task<ApiResponse<StudySessionDetailsResponse>> GetByIdAsync(string userId, Guid sessionId);

    /// <summary>
    /// Lista todas as mensagens de uma sessão do usuário autenticado.
    /// </summary>
    Task<ApiResponse<List<MessageResponse>>> GetMessagesAsync(string userId, Guid sessionId);

    /// <summary>
    /// Cria uma nova mensagem em uma sessão do usuário autenticado.
    /// </summary>
    Task<ApiResponse<MessageResponse>> CreateMessageAsync(string userId, Guid sessionId, CreateMessageRequest request);
}