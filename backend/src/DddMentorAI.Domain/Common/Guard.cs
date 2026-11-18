namespace DddMentorAI.Domain.Common;

internal static class Guard
{
    public static string Text(string? value, string field, int minLength, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{field} is required");
        }

        if (value.Length < minLength || value.Length > maxLength)
        {
            throw new DomainException($"{field} must be between {minLength} and {maxLength} characters");
        }

        return value;
    }

    public static DateTime Utc(DateTime value, string field)
    {
        if (value.Kind != DateTimeKind.Utc)
        {
            throw new DomainException($"{field} must be in UTC");
        }

        return value;
    }
}
