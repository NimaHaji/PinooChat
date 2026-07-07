namespace Application.Features.Auth.DTOs;

public class LoginUserRequestDto
{
    public string Identifier { get; set; }
    public string Password { get; set; }
}