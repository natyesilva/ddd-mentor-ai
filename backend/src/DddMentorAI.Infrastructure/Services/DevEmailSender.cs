using DddMentorAI.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace DddMentorAI.Infrastructure.Services;

/// <summary>
/// Development email sender that logs confirmation links instead of sending real emails.
/// </summary>
public class DevEmailSender : IEmailSender
{
    private readonly ILogger<DevEmailSender> _logger;

    public DevEmailSender(ILogger<DevEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string toEmail, string subject, string message)
    {
        // In development mode, log the email instead of sending it
        _logger.LogInformation("=== DEV EMAIL SENDER ===");
        _logger.LogInformation("To: {ToEmail}", toEmail);
        _logger.LogInformation("Subject: {Subject}", subject);
        _logger.LogInformation("Message: {Message}", message);
        _logger.LogInformation("=========================");

        return Task.CompletedTask;
    }
}