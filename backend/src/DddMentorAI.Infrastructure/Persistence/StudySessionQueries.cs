using DddMentorAI.Application.DTOs.Responses.StudySession;
using DddMentorAI.Application.Interfaces.Queries;
using DddMentorAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DddMentorAI.Infrastructure.Persistence;

/// <summary>
/// Leituras sem rastreamento, projetadas direto para DTO.
/// </summary>
public class StudySessionQueries : IStudySessionQueries
{
    private readonly AppDbContext _context;

    public StudySessionQueries(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<StudySessionResponse>> ListByUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var rows = await _context.StudySessions
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.UpdatedAt)
            .Select(s => new { s.Id, s.Title, s.Topic, s.Level, s.CreatedAt, s.UpdatedAt, MessageCount = s.Messages.Count })
            .ToListAsync(cancellationToken);

        return rows.Select(r => new StudySessionResponse
        {
            Id = r.Id,
            Title = r.Title.Value,
            Topic = r.Topic.Value,
            Level = r.Level,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
            MessageCount = r.MessageCount
        }).ToList();
    }

    public async Task<StudySessionDetailsResponse?> GetDetailsAsync(string userId, Guid sessionId, CancellationToken cancellationToken = default)
    {
        var row = await _context.StudySessions
            .AsNoTracking()
            .Where(s => s.Id == sessionId && s.UserId == userId)
            .Select(s => new { s.Id, s.Title, s.Topic, s.Level, s.CreatedAt, s.UpdatedAt, MessageCount = s.Messages.Count })
            .FirstOrDefaultAsync(cancellationToken);

        return row is null
            ? null
            : new StudySessionDetailsResponse
            {
                Id = row.Id,
                Title = row.Title.Value,
                Topic = row.Topic.Value,
                Level = row.Level,
                CreatedAt = row.CreatedAt,
                UpdatedAt = row.UpdatedAt,
                MessageCount = row.MessageCount
            };
    }

    public async Task<List<MessageResponse>?> GetMessagesAsync(string userId, Guid sessionId, CancellationToken cancellationToken = default)
    {
        var owned = await _context.StudySessions
            .AnyAsync(s => s.Id == sessionId && s.UserId == userId, cancellationToken);

        if (!owned)
        {
            return null;
        }

        var rows = await _context.Messages
            .AsNoTracking()
            .Where(m => m.StudySessionId == sessionId)
            .OrderBy(m => m.CreatedAt)
            .Select(m => new { m.Id, m.StudySessionId, m.Role, m.Content, m.CreatedAt })
            .ToListAsync(cancellationToken);

        return rows.Select(m => new MessageResponse
        {
            Id = m.Id,
            StudySessionId = m.StudySessionId,
            Role = m.Role,
            Content = m.Content.Value,
            CreatedAt = m.CreatedAt
        }).ToList();
    }
}
