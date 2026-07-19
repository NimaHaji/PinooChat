using System.Text.RegularExpressions;
using Application.Features.Group.DTOs;
using Application.Features.Group.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GroupController : ControllerBase
{
    private readonly GroupServiceContract _groupService;

    public GroupController(GroupServiceContract groupService)
    {
        _groupService = groupService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto dto)
    {
        await _groupService.CreateGroup(dto);
        return Ok();
    }

    [HttpGet("members")]
    public async Task<IActionResult> Members([FromQuery]Guid conversationId)
    {
        var result=await _groupService.GetMembers(conversationId);
        return Ok(result);
    }
}