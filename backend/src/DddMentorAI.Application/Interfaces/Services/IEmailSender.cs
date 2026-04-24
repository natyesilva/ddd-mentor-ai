namespace DddMentorAI.Application.Interfaces.Services;

/// <summary>
/// Abstraction for email sending operations.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Sends an email asynchronously.
    /// </summary>
    Task SendEmailAsync(string toEmail, string subject, string message);
}