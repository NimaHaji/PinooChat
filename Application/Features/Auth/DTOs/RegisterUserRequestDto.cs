using System.Reflection.PortableExecutable;

namespace Application.Features.Auth.DTOs;

public class RegisterUserRequestDto
{
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MobilePhone { get; set; }
    public string Email  { get; set; }
    public string Password { get; set; }
}