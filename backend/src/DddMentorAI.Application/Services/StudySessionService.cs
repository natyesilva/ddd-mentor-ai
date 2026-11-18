using DddMentorAI.Application.DTOs.Requests.StudySession;
using DddMentorAI.Application.DTOs.Responses;
using DddMentorAI.Application.DTOs.Responses.StudySession;
using DddMentorAI.Application.Interfaces.Persistence;
using DddMentorAI.Application.Interfaces.Queries;
using DddMentorAI.Application.Interfaces.Services;
using DddMentorAI.Domain.Common;
using DddMentorAI.Domain.Entities;
using DddMentorAI.Domain.Repositories;
using DddMentorAI.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace DddMentorAI.Application.Services;

/// <summary>
/// Casos de uso de sessão de estudo. Escritas passam pelo agregado; leituras usam projeções.
/// </summary>
public class StudySessionService : IStudySessionService
{
    private const string NotFound = "Study session not found";

    private readonly IStudySessionRepository _sessions;
    private readonly IStudySessionQueries _queries;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _clock;
    private readonly ILogger<StudySessionService> _logger;

    public StudySessionService(
        IStudySessionRepository sessions,
        IStudySessionQueries queries,
        IUnitOfWork unitOfWork,
        TimeProvider clock,
        ILogger<StudySessionService> logger)
    {
        _sessions = sessions;
        _queries = queries;
        _unitOfWork = unitOfWork;
        _clock = clock;
        _logger = logger;
    }

    public async Task<ApiResponse<StudySessionResponse>> CreateAsync(string userId, CreateStudySessionRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var session = StudySession.Start(
                userId,
                SessionTitle.From(request.Title),
                StudyTopic.From(request.Topic),
                request.Level,
                Now());

            _sessions.Add(session);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<StudySessionResponse>.SuccessResponse(ToResponse(session), "Study session created successfully");
        }
        catch (DomainException ex)
        {
            return ApiResponse<StudySessionResponse>.ErrorResponse(ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error creating study session");
            return ApiResponse<StudySessionResponse>.ErrorResponse("An error occurred while creating the study session");
        }
    }

    public async Task<ApiResponse<List<StudySessionResponse>>> GetAllAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var sessions = await _queries.ListByUserAsync(userId, cancellationToken);
            return ApiResponse<List<StudySessionResponse>>.SuccessResponse(sessions);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error retrieving study sessions");
            return ApiResponse<List<StudySessionResponse>>.ErrorResponse("An error occurred while retrieving study sessions");
        }
    }

    public async Task<ApiResponse<StudySessionDetailsResponse>> GetByIdAsync(string userId, Guid sessionId, CancellationToken cancellationToken = default)
    {
        try
        {
            var session = await _queries.GetDetailsAsync(userId, sessionId, cancellationToken);
            return session is null
                ? ApiResponse<StudySessionDetailsResponse>.ErrorResponse(NotFound)
                : ApiResponse<StudySessionDetailsResponse>.SuccessResponse(session);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error retrieving study session details");
            return ApiResponse<StudySessionDetailsResponse>.ErrorResponse("An error occurred while retrieving study session details");
        }
    }

    public async Task<ApiResponse<List<MessageResponse>>> GetMessagesAsync(string userId, Guid sessionId, CancellationToken cancellationToken = default)
    {
        try
        {
            var messages = await _queries.GetMessagesAsync(userId, sessionId, cancellationToken);
            return messages is null
                ? ApiResponse<List<MessageResponse>>.ErrorResponse(NotFound)
                : ApiResponse<List<MessageResponse>>.SuccessResponse(messages);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error retrieving messages");
            return ApiResponse<List<MessageResponse>>.ErrorResponse("An error occurred while retrieving messages");
        }
    }

    public async Task<ApiResponse<MessageResponse>> CreateMessageAsync(string userId, Guid sessionId, CreateMessageRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var session = await _sessions.GetOwnedAsync(sessionId, userId, cancellationToken);
            if (session is null)
            {
                return ApiResponse<MessageResponse>.ErrorResponse(NotFound);
            }

            var message = session.AddUserMessage(MessageContent.From(request.Content), Now());
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<MessageResponse>.SuccessResponse(ToResponse(message), "Message created successfully");
        }
        catch (DomainException ex)
        {
            return ApiResponse<MessageResponse>.ErrorResponse(ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error creating message");
            return ApiResponse<MessageResponse>.ErrorResponse("An error occurred while creating the message");
        }
    }

    private DateTime Now() => _clock.GetUtcNow().UtcDateTime;

    private static StudySessionResponse ToResponse(StudySession session) => new()
    {
        Id = session.Id,
        Title = session.Title.Value,
        Topic = session.Topic.Value,
        Level = session.Level,
        CreatedAt = session.CreatedAt,
        UpdatedAt = session.UpdatedAt,
        MessageCount = session.Messages.Count
    };

    private static MessageResponse ToResponse(Message message) => new()
    {
        Id = message.Id,
        StudySessionId = message.StudySessionId,
        Role = message.Role,
        Content = message.Content.Value,
        CreatedAt = message.CreatedAt
    };
}
