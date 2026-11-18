namespace DddMentorAI.Domain.Common;

/// <summary>
/// Violação de uma invariante do domínio. A mensagem é segura para ser exibida ao cliente.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
