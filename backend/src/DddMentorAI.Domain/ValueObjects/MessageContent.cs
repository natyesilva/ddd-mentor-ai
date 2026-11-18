using DddMentorAI.Domain.Common;

namespace DddMentorAI.Domain.ValueObjects;

/// <summary>
/// Conteúdo de uma mensagem (1 a 10.000 caracteres, não pode ser só espaços).
/// </summary>
public sealed record MessageContent
{
    public const int MinLength = 1;
    public const int MaxLength = 10_000;

    public string Value { get; }

    private MessageContent(string value) => Value = value;

    public static MessageContent From(string? value) =>
        new(Guard.Text(value, "Content", MinLength, MaxLength));

    public override string ToString() => Value;
}
