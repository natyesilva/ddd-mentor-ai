using DddMentorAI.Domain.Common;
using DddMentorAI.Domain.ValueObjects;

namespace DddMentorAI.UnitTests.Domain;

public class ValueObjectTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("a")]
    public void SessionTitle_rejects_invalid_values(string? value)
    {
        Assert.Throws<DomainException>(() => SessionTitle.From(value));
    }

    [Fact]
    public void SessionTitle_enforces_max_length()
    {
        SessionTitle.From(new string('a', SessionTitle.MaxLength));
        Assert.Throws<DomainException>(() => SessionTitle.From(new string('a', SessionTitle.MaxLength + 1)));
    }

    [Fact]
    public void StudyTopic_enforces_max_length()
    {
        StudyTopic.From(new string('a', StudyTopic.MaxLength));
        Assert.Throws<DomainException>(() => StudyTopic.From(new string('a', StudyTopic.MaxLength + 1)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  \n ")]
    public void MessageContent_rejects_blank(string? value)
    {
        Assert.Throws<DomainException>(() => MessageContent.From(value));
    }

    [Fact]
    public void MessageContent_accepts_single_character_and_max_length()
    {
        Assert.Equal("?", MessageContent.From("?").Value);
        MessageContent.From(new string('a', MessageContent.MaxLength));
        Assert.Throws<DomainException>(() => MessageContent.From(new string('a', MessageContent.MaxLength + 1)));
    }

    [Fact]
    public void Value_objects_compare_by_value()
    {
        Assert.Equal(SessionTitle.From("Agregados"), SessionTitle.From("Agregados"));
        Assert.NotEqual(SessionTitle.From("Agregados"), SessionTitle.From("Entidades"));
    }
}
