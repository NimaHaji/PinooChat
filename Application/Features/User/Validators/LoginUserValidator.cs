using Application.Features.Auth.DTOs;
using FluentValidation;

namespace Application.Features.User.Validators;

public class LoginUserValidator : AbstractValidator<LoginUserRequestDto>
{
    public LoginUserValidator()
    {

        RuleFor(x => x.Identifier)
            .NotEmpty()
            .WithMessage("شناسه کاربری (ایمیل یا نام کاربری) الزامی است.")
            .Must(BeValidIdentifier)
            .WithMessage("شناسه کاربری باید یک ایمیل معتبر یا نام کاربری معتبر باشد.");
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("رمز عبور الزامی است.")
            .MinimumLength(6)
            .WithMessage("رمز عبور باید حداقل ۶ کاراکتر باشد.");
    }
    
    private bool BeValidIdentifier(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            return false;
        
        if (identifier.Contains('@'))
        {
            return IsValidEmail(identifier);
        }
        
        return IsValidUsername(identifier);
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private bool IsValidUsername(string username)
    {
        return !string.IsNullOrWhiteSpace(username) &&
               username.Length >= 3 &&
               username.Length <= 30 &&
               System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$");
    }
}