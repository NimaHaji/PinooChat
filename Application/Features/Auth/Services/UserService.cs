using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Auth.DTOs;
using Application.Features.Auth.Interfaces;
using Application.Features.Session.DTOs;
using Application.Features.Session.Interfaces;
using Domain;
using Domain.Entities;
using Domain.Enums;
using Shared.Exceptions;

namespace Application.Features.Auth.Services;

public class UserService : UserServiceContract
{
    private readonly UserRepositoryContract _userRepositoryContract;
    private readonly UserSessionServiceContract _userSessionServiceContract;
    private readonly UserSessionRepositoryContract _userSessionRepositoryContract;
    private readonly UnitOfWorkContract _unitOfWorkContract;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IHasher _hasher;
    private readonly UserContextContract _userContextContract;

    public UserService(UserRepositoryContract repositoryContract, IPasswordHasher passwordHasher,
        UnitOfWorkContract unitOfWorkContract, UserSessionServiceContract userSessionServiceContract, IHasher hasher,
        UserSessionRepositoryContract userSessionRepositoryContract, UserContextContract userContextContract)
    {
        _userRepositoryContract = repositoryContract;
        _passwordHasher = passwordHasher;
        _unitOfWorkContract = unitOfWorkContract;
        _userSessionServiceContract = userSessionServiceContract;
        _hasher = hasher;
        _userSessionRepositoryContract = userSessionRepositoryContract;
        _userContextContract = userContextContract;
    }

    public async Task<string> RegisterUserAsync(RegisterUserRequestDto registerUserRequestDto)
    {
        if (await _userRepositoryContract.IsUserExistsByEmailAsync(registerUserRequestDto.Email))
            throw new DuplicateUserException("این ایمیل قبلا ثبت شده !");
        if (await _userRepositoryContract.IsUserExistsByUserNameAsync(registerUserRequestDto.UserName))
            throw new DuplicateUserException("این نام کاربری قبلا ثبت شده !");

        var password = _passwordHasher.Hash(registerUserRequestDto.Password);
        var user = new Domain.Entities.User(registerUserRequestDto.UserName, registerUserRequestDto.FirstName,
            registerUserRequestDto.LastName, registerUserRequestDto.Email, registerUserRequestDto.MobilePhone,
            password);
        await _userRepositoryContract.RegisterUserAsync(user);
        await _unitOfWorkContract.SaveAsync();
        return $"کاربر {user.UserName} خوش آمدید";
    }

    public async Task<LoginUserResponseDto> LoginUserAsync(LoginUserRequestDto loginUserRequestDto, string userAgent,
        string ipAddress)
    {
        var users = await _userRepositoryContract.GetUsersByEmailOrUserNameAsync(loginUserRequestDto.Identifier);

        if (users == null)
            throw new UnauthorizedAccessException("Invalid email/username or password");

        var matchedUsers = users?
            .Where(us => _passwordHasher.Verify(us.PasswordHash, loginUserRequestDto.Password))
            .ToList();
        if (matchedUsers.Count == 0)
            throw new UnauthorizedAccessException("Invalid email or password");

        if (matchedUsers.Count == 1)
        {
            var user = matchedUsers.First();

            var RefreshToken = await _userSessionServiceContract.GenerateSessionToken();
            var tokenHashed = _hasher.Hash(RefreshToken);

            var accessToken = await _userSessionServiceContract.GenerateSessionToken();
            var accessTokenHashed = _hasher.Hash(accessToken);

            var session = new UserSession(
                userId: user.Id,
                tokenHash: tokenHashed,
                expiredAtUtc: DateTime.UtcNow.AddDays(180),
                deviceName: userAgent,
                ipAddress: ipAddress,
                userAgent: userAgent,
                accessTokenHash: accessTokenHashed
            );


            await _userSessionRepositoryContract.CreateUserSessionAsync(session);
            await _unitOfWorkContract.SaveAsync();

            return new LoginUserResponseDto(RefreshToken, accessToken);
        }

        return new LoginUserResponseDto(null, null);
    }

    public async Task<RefreshTokenResponseDto> RotateTokenAsync(string refreshToken, string userAgent, string ipAddress)
    {
        var tokenHash = _hasher.Hash(refreshToken);

        var session = await _userSessionRepositoryContract
                          .GetSessionByRefreshTokenHashedAsync(tokenHash)
                      ?? throw new UnauthorizedAccessException("نشستی یافت نشد");

        if (!session.IsActive)
            throw new UnauthorizedAccessException("Refresh Token معتبر نیست.");

        var user = await _userRepositoryContract.GetUserByIdAsync(session.UserId);

        var accessToken = await _userSessionServiceContract.GenerateSessionToken();

        var newRefreshToken = await _userSessionServiceContract.GenerateSessionToken();

        var newTokenHashed = _hasher.Hash(newRefreshToken);
        var newAccessTokenHashed = _hasher.Hash(accessToken);

        var newSession = new UserSession(
            userId: user.Id,
            tokenHash: newTokenHashed,
            expiredAtUtc: session.ExpiredAtUtc,
            deviceName: userAgent,
            ipAddress: ipAddress,
            userAgent: userAgent,
            accessTokenHash: newAccessTokenHashed
        );
        session.Revoke(newSession.Id, "RotateToken");


        await _userSessionRepositoryContract.CreateUserSessionAsync(newSession);
        await _unitOfWorkContract.SaveAsync();

        return new RefreshTokenResponseDto(newRefreshToken, accessToken);
    }


    public async Task<string> LogoutUserAsync()
    {
        var userId = _userContextContract.UserId ?? throw new UnauthorizedAccessException("کاربر یافت نشد .");

        var sessions = await _userSessionRepositoryContract.GetSessionsByUserIdAsync(userId);

        if (sessions == null || !sessions.Any())
            throw new NotFoundException("نشستی برای این کاربر یافت نشد");

        foreach (var token in sessions)
        {
            token.Revoke(null, "Logout");
        }

        await _unitOfWorkContract.SaveAsync();
        return "کاربر خارج شد .";
    }

    public async Task<ProfileResponseDto> ViewProfileAsync()
    {
        var userId = _userContextContract.UserId ?? throw new UnauthorizedAccessException("کاربر احراز هویت نشده است.");
        var user = await _userRepositoryContract.GetUserByIdAsync(userId);
        return new ProfileResponseDto
        {
            Id = user.Id,
            UserName = user.UserName,
            FullName = user.FirstName + " " + user.LastName,
            Email = user.Email,
            PhoneNumber = user.MobilePhone ?? "موبایل وارد نشده",
        };
    }

    public async Task<ProfileResponseDto> UpdateProfileAsync(UpdateProfileRequestDto dto)
    {
        var userId = _userContextContract.UserId ?? throw new UnauthorizedAccessException("کاربر احراز هویت نشده است.");
        var user = await _userRepositoryContract.GetUserByIdAsync(userId)
                   ?? throw new UnauthorizedAccessException("Invalid user id");

        if (!string.IsNullOrEmpty(dto.FirstName))
        {
            user.ChangeFirstName(dto.FirstName);
        }

        if (!string.IsNullOrEmpty(dto.LastName))
        {
            user.ChangeFirstName(dto.LastName);
        }

        if (!string.IsNullOrEmpty(dto.PhoneNumber))
        {
            user.ChangeMobilePhone(dto.PhoneNumber);
        }

        await _unitOfWorkContract.SaveAsync();

        return new ProfileResponseDto()
        {
            Id = user.Id,
            UserName = user.UserName,
            FullName = user.FirstName + " " + user.LastName,
            Email = user.Email,
            PhoneNumber = user.MobilePhone ?? "موبایل وارد نشده"
        };
    }

    public async Task<List<ViewUser>> GetAllUsersAsync()
    {
        return await _userRepositoryContract.GetAllUsersAsync();
    }

    public async Task<string> PromoteUserToAdminAsync(Guid userId)
    {
        var user = await _userRepositoryContract.GetUserByIdAsync(userId) ??
                   throw new NotFoundException("کاربر یافت نشد .");
        
        user.ChangeRole(UserRole.Admin);
        
        await _unitOfWorkContract.SaveAsync();
        return $"کاربر {user.UserName} ادمین شد .";
    }
    public async Task<string> DemoteAdminToUserAsync(Guid userId)
    {
        var user = await _userRepositoryContract.GetUserByIdAsync(userId) ??
                   throw new NotFoundException("کاربر یافت نشد .");
        
        user.ChangeRole(UserRole.User);
        
        await _unitOfWorkContract.SaveAsync();
        return $"ادمین {user.UserName} کاربر شد .";
    }
}
