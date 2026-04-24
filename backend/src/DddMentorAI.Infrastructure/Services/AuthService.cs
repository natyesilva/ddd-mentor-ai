using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DddMentorAI.Application.DTOs.Requests;
using DddMentorAI.Application.DTOs.Responses;
using DddMentorAI.Application.Interfaces.Services;
using DddMentorAI.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace DddMentorAI.Infrastructure.Services;

/// <summary>
/// Implementation of authentication service with JWT support.
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<AppUser> userManager,
        IEmailSender emailSender,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return ApiResponse<RegisterResponse>.ErrorResponse("Email already registered");
            }

            // Create new user
            var user = new AppUser
            {
                UserName = request.Email,
                Email = request.Email,
                Name = request.Name,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return ApiResponse<RegisterResponse>.ErrorResponse(errors);
            }

            // Generate email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var frontendUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:5173";
            var confirmationLink = $"{frontendUrl}/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            // Send confirmation email
            var emailMessage = $@"
                <h2>Welcome to DddMentorAI!</h2>
                <p>Hello {user.Name},</p>
                <p>Thank you for registering. Please confirm your email address by clicking the link below:</p>
                <p><a href='{confirmationLink}'>Confirm Email</a></p>
                <p>Or copy and paste this link in your browser:</p>
                <p>{confirmationLink}</p>
                <p><strong>Confirmation Token (for testing):</strong> {token}</p>
                <p>If you did not create this account, please ignore this email.</p>
            ";

            await _emailSender.SendEmailAsync(user.Email, "Confirm your email - DddMentorAI", emailMessage);

            var response = new RegisterResponse
            {
                UserId = user.Id,
                Email = user.Email,
                Name = user.Name,
                Message = "Registration successful. Please check your email to confirm your account."
            };

            return ApiResponse<RegisterResponse>.SuccessResponse(response, "User registered successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            return ApiResponse<RegisterResponse>.ErrorResponse("An error occurred during registration");
        }
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return ApiResponse<LoginResponse>.ErrorResponse("Invalid email or password");
            }

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return ApiResponse<LoginResponse>.ErrorResponse("Invalid email or password");
            }

            // Check if email is confirmed
            if (!user.EmailConfirmed)
            {
                return ApiResponse<LoginResponse>.ErrorResponse(
                    "Email not confirmed. Please check your email and confirm your account, or request a new confirmation link.");
            }

            var token = await GenerateJwtTokenAsync(user);

            var response = new LoginResponse
            {
                UserId = user.Id,
                Email = user.Email,
                Name = user.Name,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(
                    _configuration.GetValue<int>("Jwt:ExpirationInMinutes", 60))
            };

            return ApiResponse<LoginResponse>.SuccessResponse(response, "Login successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user login");
            return ApiResponse<LoginResponse>.ErrorResponse("An error occurred during login");
        }
    }

    public async Task<ApiResponse<string>> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
            {
                return ApiResponse<string>.ErrorResponse("Invalid user");
            }

            var result = await _userManager.ConfirmEmailAsync(user, request.Token);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return ApiResponse<string>.ErrorResponse($"Invalid token: {errors}");
            }

            return ApiResponse<string>.SuccessResponse("Email confirmed successfully", "Email confirmed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during email confirmation");
            return ApiResponse<string>.ErrorResponse("An error occurred during email confirmation");
        }
    }

    public async Task<ApiResponse<string>> ResendConfirmationAsync(ResendConfirmationRequest request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                // Don't reveal if user exists
                return ApiResponse<string>.SuccessResponse(
                    "If the email is registered, a confirmation link has been sent");
            }

            if (user.EmailConfirmed)
            {
                return ApiResponse<string>.ErrorResponse("Email is already confirmed");
            }

            // Generate new confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var frontendUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:5173";
            var confirmationLink = $"{frontendUrl}/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            var emailMessage = $@"
                <h2>Resend Confirmation - DddMentorAI</h2>
                <p>Hello {user.Name},</p>
                <p>Please confirm your email address by clicking the link below:</p>
                <p><a href='{confirmationLink}'>Confirm Email</a></p>
                <p>Or copy and paste this link in your browser:</p>
                <p>{confirmationLink}</p>
                <p><strong>Confirmation Token (for testing):</strong> {token}</p>
            ";

            await _emailSender.SendEmailAsync(user.Email, "Confirm your email - DddMentorAI", emailMessage);

            return ApiResponse<string>.SuccessResponse(
                "If the email is registered and not confirmed, a confirmation link has been sent");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during confirmation resend");
            return ApiResponse<string>.ErrorResponse("An error occurred while resending confirmation");
        }
    }

    private async Task<string> GenerateJwtTokenAsync(AppUser user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "ThisIsASecretKeyForJwtTokenGeneration12345"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expirationMinutes = _configuration.GetValue<int>("Jwt:ExpirationInMinutes", 60);
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "DddMentorAI",
            audience: _configuration["Jwt:Audience"] ?? "DddMentorAI",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}