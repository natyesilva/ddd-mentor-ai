using DddMentorAI.Application.DTOs.Requests;
using DddMentorAI.Application.DTOs.Responses;
using DddMentorAI.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace DddMentorAI.API.Controllers;

/// <summary>
/// Controller for authentication operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Register a new user.
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<RegisterResponse>>> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<RegisterResponse>.ErrorResponse("Invalid request data"));
        }

        var result = await _authService.RegisterAsync(request);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Login with email and password.
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<LoginResponse>.ErrorResponse("Invalid request data"));
        }

        var result = await _authService.LoginAsync(request);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Confirm email address.
    /// </summary>
    [HttpPost("confirm-email")]
    public async Task<ActionResult<ApiResponse<string>>> ConfirmEmail([FromBody] ConfirmEmailRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse("Invalid request data"));
        }

        var result = await _authService.ConfirmEmailAsync(request);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Resend email confirmation.
    /// </summary>
    [HttpPost("resend-confirmation")]
    public async Task<ActionResult<ApiResponse<string>>> ResendConfirmation([FromBody] ResendConfirmationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse("Invalid request data"));
        }

        var result = await _authService.ResendConfirmationAsync(request);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}