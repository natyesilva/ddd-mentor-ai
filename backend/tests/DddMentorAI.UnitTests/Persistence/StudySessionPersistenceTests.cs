using DddMentorAI.Domain.Entities;
using DddMentorAI.Domain.Enums;
using DddMentorAI.Domain.ValueObjects;
using DddMentorAI.Infrastructure.Data;
using DddMentorAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DddMentorAI.UnitTests.Persistence;

/// <summary>
/// Valida o mapeamento EF do agregado (change tracking, campo _messages, conversões de value objects).
/// Usa o provider InMemory: não cobre SQL nem constraints do PostgreSQL.
/// </summary>
public class StudySessionPersistenceTests
{
    private static readonly DateTime Now = new(2026, 11, 17, 12, 0, 0, DateTimeKind.Utc);

    private static AppDbContext NewContext(string database) =>
        new(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(database).Options);

    [Fact]
    public async Task Message_added_to_loaded_aggregate_is_inserted()
    {
        var database = Guid.NewGuid().ToString();
        Guid sessionId;

        await using (var context = NewContext(database))
        {
            var session = StudySession.Start("user-1", SessionTitle.From("Agregados"), StudyTopic.From("Fronteiras"), StudyLevel.Beginner, Now);
            new StudySessionRepository(context).Add(session);
            await context.SaveChangesAsync();
            sessionId = session.Id;
        }

        await using (var context = NewContext(database))
        {
            var session = await new StudySessionRepository(context).GetOwnedAsync(sessionId, "user-1");
            var message = session!.AddUserMessage(MessageContent.From("O que é um agregado?"), Now.AddMinutes(1));

            // Sem ValueGeneratedNever no Id, o EF marcaria como Modified e o SaveChanges geraria UPDATE.
            context.ChangeTracker.DetectChanges();
            Assert.Equal(EntityState.Added, context.Entry(message).State);
            await context.SaveChangesAsync();
        }

        await using (var context = NewContext(database))
        {
            var messages = await new StudySessionQueries(context).GetMessagesAsync("user-1", sessionId);
            var message = Assert.Single(messages!);
            Assert.Equal("O que é um agregado?", message.Content);

            var details = await new StudySessionQueries(context).GetDetailsAsync("user-1", sessionId);
            Assert.Equal(1, details!.MessageCount);
            Assert.Equal(Now.AddMinutes(1), details.UpdatedAt);
        }
    }

    [Fact]
    public async Task Repository_and_queries_filter_by_owner()
    {
        var database = Guid.NewGuid().ToString();
        await using var context = NewContext(database);
        var session = StudySession.Start("owner", SessionTitle.From("Agregados"), StudyTopic.From("Fronteiras"), StudyLevel.Beginner, Now);
        context.StudySessions.Add(session);
        await context.SaveChangesAsync();

        Assert.Null(await new StudySessionRepository(context).GetOwnedAsync(session.Id, "intruder"));
        Assert.Null(await new StudySessionQueries(context).GetMessagesAsync("intruder", session.Id));
        Assert.Null(await new StudySessionQueries(context).GetDetailsAsync("intruder", session.Id));
        Assert.Empty(await new StudySessionQueries(context).ListByUserAsync("intruder"));
    }
}
