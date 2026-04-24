using Microsoft.AspNetCore.Identity;

namespace DddMentorAI.Infrastructure.Identity;

/// <summary>
/// Custom user entity extending IdentityUser for application-specific properties.
/// </summary>
public class AppUser : IdentityUser
{
    /// <summary>
    /// User's full name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the user account was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}