using DddMentorAI.Domain.Common;

namespace DddMentorAI.Domain.ValueObjects;

/// <summary>
/// Título de uma sessão de estudo (2 a 200 caracteres, não vazio).
/// </summary>
public sealed record SessionTitle
{
    public const int MinLength = 2;
    public const int MaxLength = 200;

    public string Value { get; }

    private SessionTitle(string value) => Value = value;

    public static SessionTitle From(string? value) =>
        new(Guard.Text(value, "Title", MinLength, MaxLength));

    public override string ToString() => Value;
}
