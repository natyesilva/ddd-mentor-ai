namespace DddMentorAI.Application.Interfaces.Persistence;

/// <summary>
/// Confirma as alterações feitas nos agregados carregados na requisição.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
