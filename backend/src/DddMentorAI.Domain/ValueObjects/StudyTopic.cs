using DddMentorAI.Domain.Common;

namespace DddMentorAI.Domain.ValueObjects;

/// <summary>
/// Tópico de DDD estudado na sessão (2 a 500 caracteres, não vazio).
/// </summary>
public sealed record StudyTopic
{
    public const int MinLength = 2;
    public const int MaxLength = 500;

    public string Value { get; }

    private StudyTopic(string value) => Value = value;

    public static StudyTopic From(string? value) =>
        new(Guard.Text(value, "Topic", MinLength, MaxLength));

    public override string ToString() => Value;
}
