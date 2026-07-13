using System.Security.Claims;
using Application.Features.Conversation.DTOs;
using Application.Features.Conversation.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ConversationController : ControllerBase
{
    private readonly ConversationServiceContract _conversationServiceContract;

    public ConversationController(ConversationServiceContract conversationServiceContract)
    {
        _conversationServiceContract = conversationServiceContract;
    }

    [HttpGet("Conversations")]
    public async Task<IActionResult> GetConversations()
    {
        var conversation = await _conversationServiceContract.GetConversationsAsync();
        return Ok(conversation);
    }

    [HttpGet("{conversationId:guid}/messages")]
    public async Task<IActionResult> GetMessage(Guid conversationId, [FromQuery] int page = 1, [FromQuery] int pageSize = 30)
    {

        if (page < 1 || pageSize < 1 || pageSize > 100)
        {
            return BadRequest("پارامترهای صفحه‌بندی نامعتبر هستند.");
        }

        try
        {
            var messages = await _conversationServiceContract.GetConversationMessagesAsync(conversationId, page, pageSize);
            return Ok(messages);
        }
        catch (UnauthorizedAccessException)
        {
            return StatusCode(403, "شما دسترسی به این گفتگو را ندارید.");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("گفتگوی مورد نظر یافت نشد.");
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst("sub")?.Value;

        return Guid.TryParse(userIdClaim, out var parsedGuid) ? parsedGuid : Guid.Empty;
    }
}