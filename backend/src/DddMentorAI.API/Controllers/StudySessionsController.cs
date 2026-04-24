using DddMentorAI.Application.DTOs.Requests.StudySession;
using DddMentorAI.Application.DTOs.Responses;
using DddMentorAI.Application.DTOs.Responses.StudySession;
using DddMentorAI.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DddMentorAI.API.Controllers;

/// <summary>
/// Controller para operações de sessões de estudo.
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
    /// Lista todas as sessões de estudo do usuário autenticado.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<StudySessionResponse>>>> GetAll()
    {
        var userId = GetUserId();
        var result = await _studySessionService.GetAllAsync(userId);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Cria uma nova sessão de estudo.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<StudySessionResponse>>> Create([FromBody] CreateStudySessionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<StudySessionResponse>.ErrorResponse("Invalid request data"));
        }

        var userId = GetUserId();
        var result = await _studySessionService.CreateAsync(userId, request);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtém os detalhes de uma sessão de estudo específica.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<StudySessionDetailsResponse>>> GetById(Guid id)
    {
        var userId = GetUserId();
        var result = await _studySessionService.GetByIdAsync(userId, id);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Lista todas as mensagens de uma sessão de estudo.
    /// </summary>
    [HttpGet("{id:guid}/messages")]
    public async Task<ActionResult<ApiResponse<List<MessageResponse>>>> GetMessages(Guid id)
    {
        var userId = GetUserId();
        var result = await _studySessionService.GetMessagesAsync(userId, id);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Cria uma nova mensagem em uma sessão de estudo.
    /// </summary>
    [HttpPost("{id:guid}/messages")]
    public async Task<ActionResult<ApiResponse<MessageResponse>>> CreateMessage(Guid id, [FromBody] CreateMessageRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<MessageResponse>.ErrorResponse("Invalid request data"));
        }

        var userId = GetUserId();
        var result = await _studySessionService.CreateMessageAsync(userId, id, request);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    private string GetUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }
}