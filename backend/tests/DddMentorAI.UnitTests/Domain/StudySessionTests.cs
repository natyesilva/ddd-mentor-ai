using DddMentorAI.Domain.Common;
using DddMentorAI.Domain.Entities;
using DddMentorAI.Domain.Enums;
using DddMentorAI.Domain.ValueObjects;

namespace DddMentorAI.UnitTests.Domain;

public class StudySessionTests
{
    private static readonly DateTime Now = new(2026, 11, 17, 12, 0, 0, DateTimeKind.Utc);

    private static StudySession NewSession(string userId = "user-1") =>
        StudySession.Start(userId, SessionTitle.From("Agregados"), StudyTopic.From("Invariantes e fronteiras"), StudyLevel.Beginner, Now);

    [Fact]
    public void Start_sets_owner_data_and_timestamps()
    {
        var session = NewSession();

        Assert.NotEqual(Guid.Empty, session.Id);
        Assert.Equal("user-1", session.UserId);
        Assert.Equal("Agregados", session.Title.Value);
        Assert.Equal(Now, session.CreatedAt);
        Assert.Equal(Now, session.UpdatedAt);
        Assert.Empty(session.Messages);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Start_requires_an_owner(string userId)
    {
        Assert.Throws<DomainException>(() => NewSession(userId));
    }

    [Fact]
    public void Start_rejects_undefined_level()
    {
        Assert.Throws<DomainException>(() =>
            StudySession.Start("user-1", SessionTitle.From("Agregados"), StudyTopic.From("Fronteiras"), (StudyLevel)99, Now));
    }

    [Fact]
    public void Start_rejects_non_utc_time()
    {
        var local = new DateTime(2026, 11, 17, 12, 0, 0, DateTimeKind.Local);

        Assert.Throws<DomainException>(() =>
            StudySession.Start("user-1", SessionTitle.From("Agregados"), StudyTopic.From("Fronteiras"), StudyLevel.Beginner, local));
    }

    [Fact]
    public void AddUserMessage_appends_message_and_touches_session()
    {
        var session = NewSession();
        var later = Now.AddMinutes(5);

        var message = session.AddUserMessage(MessageContent.From("O que é um agregado?"), later);

        Assert.Single(session.Messages);
        Assert.Equal(session.Id, message.StudySessionId);
        Assert.Equal(MessageRole.User, message.Role);
        Assert.Equal(later, message.CreatedAt);
        Assert.Equal(later, session.UpdatedAt);
        Assert.Equal(Now, session.CreatedAt);
    }

    [Fact]
    public void Messages_cannot_be_modified_from_outside_the_aggregate()
    {
        var session = NewSession();

        Assert.False(session.Messages is List<Message>);
        Assert.Throws<NotSupportedException>(() =>
            ((ICollection<Message>)session.Messages).Add(null!));
    }

    [Fact]
    public void IsOwnedBy_compares_owner()
    {
        var session = NewSession("user-1");

        Assert.True(session.IsOwnedBy("user-1"));
        Assert.False(session.IsOwnedBy("user-2"));
    }
}
