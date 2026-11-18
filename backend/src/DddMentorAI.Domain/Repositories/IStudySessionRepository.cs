using DddMentorAI.Domain.Entities;

namespace DddMentorAI.Domain.Repositories;

/// <summary>
/// Persistência do agregado <see cref="StudySession"/>. Usado só no lado de escrita;
/// leituras usam projeções (ver IStudySessionQueries na Application).
/// </summary>
public interface IStudySessionRepository
{
    /// <summary>
    /// Carrega a sessão completa (com mensagens) se ela existir e pertencer ao usuário.
    /// </summary>
    Task<StudySession?> GetOwnedAsync(Guid sessionId, string userId, CancellationToken cancellationToken = default);

    void Add(StudySession session);
}
