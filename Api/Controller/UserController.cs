using Application.Features.Auth.DTOs;
using Application.Features.Auth.Interfaces;
using Application.Features.User.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserServiceContract _userServiceContract;

    public UserController(UserServiceContract userServiceContract)
    {
        _userServiceContract = userServiceContract;
    }

    #region Authentication
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequestDto registerUserRequestDto)
    {
        var res = await _userServiceContract.RegisterUserAsync(registerUserRequestDto);

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

        var result = await _userServiceContract.LoginUserAsync(requestDto, userAgent, ipAddress);
            
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
        
        var result = await _userServiceContract.RotateTokenAsync(refreshToken,userAgent,ipAddress);

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
        var res=await _userServiceContract.LogoutUserAsync();
        return Ok(res);
    }
    #endregion
    
    [HttpGet("ViewUsers")]
    [Authorize(Roles =  "Admin")]
    public async Task<IActionResult> ViewUsers()
    {
        var res= await _userServiceContract.GetAllUsersAsync();
        return Ok(res);
    }

    #region Search
    [HttpGet("Search")]
    public async Task<IActionResult> Search([FromQuery] string UserName)
    {
        var user=await _userServiceContract.GetUserByUserName(UserName);
        return Ok(user);
    }

    #endregion
    
    #region Profile
    [HttpGet("Profile")]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var profile = await _userServiceContract.ViewProfileAsync();
        return Ok(profile);
    }
    
    [HttpPatch("Profile")]
    [Authorize]
    public async Task<ProfileResponseDto> UpdateProfile([FromBody] UpdateProfileRequestDto updateProfileRequestDto)
    {
        return await _userServiceContract.UpdateProfileAsync(updateProfileRequestDto);
    }
    #endregion

    #region ChangeRole
    [HttpPatch("Promote/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Promote(Guid userId)
    {
       var result= await _userServiceContract.PromoteUserToAdminAsync(userId);
       return Ok(result);
    }
    
    [HttpPatch("Demote/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Demote(Guid userId)
    {
        var result= await _userServiceContract.DemoteAdminToUserAsync(userId);
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
