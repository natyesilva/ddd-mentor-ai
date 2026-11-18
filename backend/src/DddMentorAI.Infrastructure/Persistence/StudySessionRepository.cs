using DddMentorAI.Domain.Entities;
using DddMentorAI.Domain.Repositories;
using DddMentorAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DddMentorAI.Infrastructure.Persistence;

public class StudySessionRepository : IStudySessionRepository
{
    private readonly AppDbContext _context;

    public StudySessionRepository(AppDbContext context)
    {
        _context = context;
    }

    // Carrega o agregado inteiro. Trade-off aceito (ADR 0001): sessões de chat são pequenas
    // e o histórico completo será necessário como contexto do mentor de IA.
    public Task<StudySession?> GetOwnedAsync(Guid sessionId, string userId, CancellationToken cancellationToken = default) =>
        _context.StudySessions
            .Include(s => s.Messages)
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId, cancellationToken);

    public void Add(StudySession session) => _context.StudySessions.Add(session);
}
