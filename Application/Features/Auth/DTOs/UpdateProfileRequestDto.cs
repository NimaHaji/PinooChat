namespace Application.Features.Auth.DTOs;

public class UpdateProfileRequestDto
{
    public string? FirstName { get; set; } = null!;
    public string? LastName { get; set; } = null!;
    public string? PhoneNumber { get; set; } 
}