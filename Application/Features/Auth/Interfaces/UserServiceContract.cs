using Application.Features.Auth.DTOs;
using Application.Features.Session.DTOs;
using Application.Features.User.DTOs;
using Domain.Entities;

namespace Application.Features.Auth.Interfaces;

public interface UserServiceContract
{
    Task<string> RegisterUserAsync(RegisterUserRequestDto registerUserRequestDto);
    Task<LoginUserResponseDto> LoginUserAsync(LoginUserRequestDto loginUserRequestDto,string userAgent,string ipAddress);
    Task<string> LogoutUserAsync();
    Task<RefreshTokenResponseDto> RotateTokenAsync(string token,string userAgent,string ipAddress);
    Task<ProfileResponseDto> ViewProfileAsync();
    Task<ProfileResponseDto> UpdateProfileAsync(UpdateProfileRequestDto updateProfileRequestDto);
    Task<List<ViewUser>> GetAllUsersAsync();
    Task<string> PromoteUserToAdminAsync(Guid userId);
    Task<string> DemoteAdminToUserAsync(Guid userId);
    Task<List<SearchMatchedUsersDto>> GetUserByUserName(string username);
}