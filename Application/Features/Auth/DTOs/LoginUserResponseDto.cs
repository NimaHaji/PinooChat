namespace Application.Features.Auth.DTOs;

public record LoginUserResponseDto(
    string refreshToken,
    string accessToken);

