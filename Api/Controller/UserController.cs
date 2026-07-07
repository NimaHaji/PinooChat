using Application.Features.Auth.DTOs;
using Application.Features.Auth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserServiceContract _serviceContract;

    public UserController(UserServiceContract serviceContract)
    {
        _serviceContract = serviceContract;
    }

    #region Authentication
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequestDto registerUserRequestDto)
    {
        var res = await _serviceContract.RegisterUserAsync(registerUserRequestDto);

        var loginDto = new LoginUserRequestDto()
        {
            Identifier = registerUserRequestDto.Email,
            Password = registerUserRequestDto.Password,
        };

        var loginRes = await Login(loginDto) as OkObjectResult;
        
        return Ok(new
        {
            data = res,
            login = loginRes?.Value
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequestDto requestDto)
    {
        var userAgent = Request.Headers["User-Agent"].ToString();
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        var result = await _serviceContract.LoginUserAsync(requestDto, userAgent, ipAddress);
            
        SetSessionCookie(result.accessToken);
        
        return Ok(new
        {
            message = "ورود با موفقیت انجام شد",
            accesstoken = result.accessToken,
            refereshToken = result.refreshToken
        });
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody]string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Unauthorized(new { message = "Refresh Token is required" });
        
        var userAgent = Request.Headers["User-Agent"].ToString();
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        
        var result = await _serviceContract.RotateTokenAsync(refreshToken,userAgent,ipAddress);

        SetSessionCookie(result.RefreshToken);

        return Ok(new
        {
            accessToken = result.AccessToken,
            refreshToken = result.RefreshToken
        });
    }
    
    [HttpPost("Logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var res=await _serviceContract.LogoutUserAsync();
        return Ok(res);
    }
    #endregion
    
    [HttpGet("ViewUsers")]
    [Authorize(Roles =  "Admin")]
    public async Task<IActionResult> ViewUsers()
    {
        var res= await _serviceContract.GetAllUsersAsync();
        return Ok(res);
    }
    
    #region Profile
    [HttpGet("Profile")]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var profile = await _serviceContract.ViewProfileAsync();
        return Ok(profile);
    }
    
    [HttpPatch("Profile")]
    [Authorize]
    public async Task<ProfileResponseDto> UpdateProfile([FromBody] UpdateProfileRequestDto updateProfileRequestDto)
    {
        return await _serviceContract.UpdateProfileAsync(updateProfileRequestDto);
    }
    #endregion

    #region ChangeRole
    [HttpPatch("Promote/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Promote(Guid userId)
    {
       var result= await _serviceContract.PromoteUserToAdminAsync(userId);
       return Ok(result);
    }
    
    [HttpPatch("Demote/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Demote(Guid userId)
    {
        var result= await _serviceContract.DemoteAdminToUserAsync(userId);
        return Ok(result);
    }
    #endregion
    
    #region  Cookie
    private void SetSessionCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(1),
            Path = "/",
        };

        HttpContext.Response.Cookies.Append("SATH", token, cookieOptions);
    }
    #endregion
}
