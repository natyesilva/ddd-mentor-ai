using DddMentorAI.Application.DTOs.Requests;
using DddMentorAI.Application.DTOs.Responses;
using DddMentorAI.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DddMentorAI.API.Controllers;

/// <summary>
/// Controller for study session operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudySessionsController : ControllerBase
{
    private readonly IStudySessionService _studySessionService;

    public StudySessionsController(IStudySessionService studySessionService)
    {
        _studySessionService = studySessionService;
    }

    /// <summary>
    /// Get all study sessions for the authenticated user.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<StudySessionResponse>>>> GetAll()
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(ApiResponse<List<StudySessionResponse>>.ErrorResponse("Invalid user"));
        }

        var result = await _studySessionService.GetAllAsync(userId);
        return Ok(result);
    }

    /// <summary>
    /// Create a new study session.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<StudySessionResponse>>> Create([FromBody] CreateStudySessionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<StudySessionResponse>.ErrorResponse("Invalid request data"));
        }

        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(ApiResponse<StudySessionResponse>.ErrorResponse("Invalid user"));
        }

        var result = await _studySessionService.CreateAsync(userId, request);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get a study session by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<StudySessionDetailsResponse>>> GetById(Guid id)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(ApiResponse<StudySessionDetailsResponse>.ErrorResponse("Invalid user"));
        }

        var result = await _studySessionService.GetByIdAsync(userId, id);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get all messages for a study session.
    /// </summary>
    [HttpGet("{id:guid}/messages")]
    public async Task<ActionResult<ApiResponse<List<MessageResponse>>>> GetMessages(Guid id)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(ApiResponse<List<MessageResponse>>.ErrorResponse("Invalid user"));
        }

        var result = await _studySessionService.GetMessagesAsync(userId, id);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Create a new message in a study session.
    /// </summary>
    [HttpPost("{id:guid}/messages")]
    public async Task<ActionResult<ApiResponse<MessageResponse>>> CreateMessage(Guid id, [FromBody] CreateMessageRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<MessageResponse>.ErrorResponse("Invalid request data"));
        }

        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(ApiResponse<MessageResponse>.ErrorResponse("Invalid user"));
        }

        var result = await _studySessionService.CreateMessageAsync(userId, id, request);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    private string? GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}