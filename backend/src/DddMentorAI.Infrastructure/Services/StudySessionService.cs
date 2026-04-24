using DddMentorAI.Application.DTOs.Requests.StudySession;
using DddMentorAI.Application.DTOs.Responses;
using DddMentorAI.Application.DTOs.Responses.StudySession;
using DddMentorAI.Application.Interfaces.Services;
using DddMentorAI.Domain.Entities;
using DddMentorAI.Domain.Enums;
using DddMentorAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DddMentorAI.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de sessões de estudo.
/// </summary>
public class StudySessionService : IStudySessionService
{
    private readonly AppDbContext _context;
    private readonly ILogger<StudySessionService> _logger;

    public StudySessionService(AppDbContext context, ILogger<StudySessionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponse<StudySessionResponse>> CreateAsync(string userId, CreateStudySessionRequest request)
    {
        try
        {
            var session = new StudySession
            {
                UserId = userId,
                Title = request.Title,
                Topic = request.Topic,
                Level = request.Level,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.StudySessions.Add(session);
            await _context.SaveChangesAsync();

            var response = new StudySessionResponse
            {
                Id = session.Id,
                Title = session.Title,
                Topic = session.Topic,
                Level = session.Level,
                CreatedAt = session.CreatedAt,
                UpdatedAt = session.UpdatedAt,
                MessageCount = 0
            };

            return ApiResponse<StudySessionResponse>.SuccessResponse(response, "Study session created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating study session");
            return ApiResponse<StudySessionResponse>.ErrorResponse("An error occurred while creating the study session");
        }
    }

    public async Task<ApiResponse<List<StudySessionResponse>>> GetAllAsync(string userId)
    {
        try
        {
            var sessions = await _context.StudySessions
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.UpdatedAt)
                .Select(s => new StudySessionResponse
                {
                    Id = s.Id,
                    Title = s.Title,
                    Topic = s.Topic,
                    Level = s.Level,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    MessageCount = s.Messages.Count
                })
                .ToListAsync();

            return ApiResponse<List<StudySessionResponse>>.SuccessResponse(sessions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving study sessions");
            return ApiResponse<List<StudySessionResponse>>.ErrorResponse("An error occurred while retrieving study sessions");
        }
    }

    public async Task<ApiResponse<StudySessionDetailsResponse>> GetByIdAsync(string userId, Guid sessionId)
    {
        try
        {
            var session = await _context.StudySessions
                .Where(s => s.Id == sessionId && s.UserId == userId)
                .Select(s => new StudySessionDetailsResponse
                {
                    Id = s.Id,
                    Title = s.Title,
                    Topic = s.Topic,
                    Level = s.Level,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    MessageCount = s.Messages.Count
                })
                .FirstOrDefaultAsync();

            if (session == null)
            {
                return ApiResponse<StudySessionDetailsResponse>.ErrorResponse("Study session not found");
            }

            return ApiResponse<StudySessionDetailsResponse>.SuccessResponse(session);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving study session details");
            return ApiResponse<StudySessionDetailsResponse>.ErrorResponse("An error occurred while retrieving study session details");
        }
    }

    public async Task<ApiResponse<List<MessageResponse>>> GetMessagesAsync(string userId, Guid sessionId)
    {
        try
        {
            // Verify that the session belongs to the user
            var sessionExists = await _context.StudySessions
                .AnyAsync(s => s.Id == sessionId && s.UserId == userId);

            if (!sessionExists)
            {
                return ApiResponse<List<MessageResponse>>.ErrorResponse("Study session not found");
            }

            var messages = await _context.Messages
                .Where(m => m.StudySessionId == sessionId)
                .OrderBy(m => m.CreatedAt)
                .Select(m => new MessageResponse
                {
                    Id = m.Id,
                    StudySessionId = m.StudySessionId,
                    Role = m.Role,
                    Content = m.Content,
                    CreatedAt = m.CreatedAt
                })
                .ToListAsync();

            return ApiResponse<List<MessageResponse>>.SuccessResponse(messages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving messages");
            return ApiResponse<List<MessageResponse>>.ErrorResponse("An error occurred while retrieving messages");
        }
    }

    public async Task<ApiResponse<MessageResponse>> CreateMessageAsync(string userId, Guid sessionId, CreateMessageRequest request)
    {
        try
        {
            // Verify that the session belongs to the user
            var session = await _context.StudySessions
                .FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId);

            if (session == null)
            {
                return ApiResponse<MessageResponse>.ErrorResponse("Study session not found");
            }

            var message = new Message
            {
                StudySessionId = sessionId,
                Role = MessageRole.User,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            
            // Update the session's UpdatedAt
            session.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();

            var response = new MessageResponse
            {
                Id = message.Id,
                StudySessionId = message.StudySessionId,
                Role = message.Role,
                Content = message.Content,
                CreatedAt = message.CreatedAt
            };

            return ApiResponse<MessageResponse>.SuccessResponse(response, "Message created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating message");
            return ApiResponse<MessageResponse>.ErrorResponse("An error occurred while creating the message");
        }
    }
}