namespace Application.Features.Session.DTOs;

public record RefreshTokenResponseDto(
    string RefreshToken,
    string AccessToken);