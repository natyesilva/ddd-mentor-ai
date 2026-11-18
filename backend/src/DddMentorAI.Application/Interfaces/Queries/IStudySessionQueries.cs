using DddMentorAI.Application.DTOs.Responses.StudySession;

namespace DddMentorAI.Application.Interfaces.Queries;

/// <summary>
/// Lado de leitura das sessões: projeções direto para DTO, sem carregar o agregado.
/// Todas as consultas já filtram pelo dono da sessão.
/// </summary>
public interface IStudySessionQueries
{
    Task<List<StudySessionResponse>> ListByUserAsync(string userId, CancellationToken cancellationToken = default);

    Task<StudySessionDetailsResponse?> GetDetailsAsync(string userId, Guid sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retorna null quando a sessão não existe ou não pertence ao usuário.
    /// </summary>
    Task<List<MessageResponse>?> GetMessagesAsync(string userId, Guid sessionId, CancellationToken cancellationToken = default);
}
