using DddMentorAI.Application.DTOs.Requests.StudySession;
using DddMentorAI.Application.DTOs.Responses.StudySession;
using DddMentorAI.Application.Interfaces.Persistence;
using DddMentorAI.Application.Interfaces.Queries;
using DddMentorAI.Application.Services;
using DddMentorAI.Domain.Entities;
using DddMentorAI.Domain.Enums;
using DddMentorAI.Domain.Repositories;
using DddMentorAI.Domain.ValueObjects;
using Microsoft.Extensions.Logging.Abstractions;

namespace DddMentorAI.UnitTests.Application;

public class StudySessionServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 17, 12, 0, 0, TimeSpan.Zero);

    private readonly FakeRepository _repository = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly FixedClock _clock = new(Now);

    private StudySessionService CreateService() =>
        new(_repository, new NoQueries(), _unitOfWork, _clock, NullLogger<StudySessionService>.Instance);

    [Fact]
    public async Task CreateAsync_persists_session_using_clock()
    {
        var result = await CreateService().CreateAsync("user-1",
            new CreateStudySessionRequest { Title = "Agregados", Topic = "Fronteiras", Level = StudyLevel.Intermediate });

        Assert.True(result.Success);
        Assert.Equal(1, _unitOfWork.Saves);
        var saved = Assert.Single(_repository.Sessions);
        Assert.Equal(Now.UtcDateTime, saved.CreatedAt);
        Assert.Equal(saved.Id, result.Data!.Id);
    }

    [Fact]
    public async Task CreateAsync_returns_domain_error_without_saving()
    {
        var result = await CreateService().CreateAsync("user-1",
            new CreateStudySessionRequest { Title = "   ", Topic = "Fronteiras", Level = StudyLevel.Beginner });

        Assert.False(result.Success);
        Assert.Equal("Title is required", result.Message);
        Assert.Equal(0, _unitOfWork.Saves);
        Assert.Empty(_repository.Sessions);
    }

    [Fact]
    public async Task CreateMessageAsync_adds_message_and_updates_session()
    {
        var session = _repository.Seed("user-1", Now.UtcDateTime.AddHours(-1));

        var result = await CreateService().CreateMessageAsync("user-1", session.Id,
            new CreateMessageRequest { Content = "O que é um agregado?" });

        Assert.True(result.Success);
        Assert.Equal(1, _unitOfWork.Saves);
        Assert.Single(session.Messages);
        Assert.Equal(Now.UtcDateTime, session.UpdatedAt);
        Assert.Equal(MessageRole.User, result.Data!.Role);
    }

    [Fact]
    public async Task CreateMessageAsync_hides_sessions_of_other_users()
    {
        var session = _repository.Seed("owner", Now.UtcDateTime);

        var result = await CreateService().CreateMessageAsync("intruder", session.Id,
            new CreateMessageRequest { Content = "oi" });

        Assert.False(result.Success);
        Assert.Equal("Study session not found", result.Message);
        Assert.Empty(session.Messages);
        Assert.Equal(0, _unitOfWork.Saves);
    }

    private sealed class FakeRepository : IStudySessionRepository
    {
        public List<StudySession> Sessions { get; } = new();

        public StudySession Seed(string userId, DateTime createdAt)
        {
            var session = StudySession.Start(userId, SessionTitle.From("Agregados"), StudyTopic.From("Fronteiras"), StudyLevel.Beginner, createdAt);
            Sessions.Add(session);
            return session;
        }

        public Task<StudySession?> GetOwnedAsync(Guid sessionId, string userId, CancellationToken cancellationToken = default) =>
            Task.FromResult(Sessions.FirstOrDefault(s => s.Id == sessionId && s.IsOwnedBy(userId)));

        public void Add(StudySession session) => Sessions.Add(session);
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int Saves { get; private set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            Saves++;
            return Task.FromResult(1);
        }
    }

    private sealed class FixedClock : TimeProvider
    {
        private readonly DateTimeOffset _now;

        public FixedClock(DateTimeOffset now) => _now = now;

        public override DateTimeOffset GetUtcNow() => _now;
    }

    private sealed class NoQueries : IStudySessionQueries
    {
        public Task<List<StudySessionResponse>> ListByUserAsync(string userId, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<StudySessionDetailsResponse?> GetDetailsAsync(string userId, Guid sessionId, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<List<MessageResponse>?> GetMessagesAsync(string userId, Guid sessionId, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }
}
