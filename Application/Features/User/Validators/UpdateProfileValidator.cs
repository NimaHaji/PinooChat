using Application.Features.Auth.DTOs;
using FluentValidation;

namespace Application.Features.User.Validators;

public class UpdateProfileValidator:AbstractValidator<UpdateProfileRequestDto>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.FirstName)
            .Cascade(CascadeMode.Stop)
            .MinimumLength(3)
            .WithMessage("نام باید حداقل 3 کاراکتر باشد.")
            .MaximumLength(50)
            .WithMessage("نام نمی‌تواند بیشتر از 50 کاراکتر باشد.")
            .When(x=>!string.IsNullOrEmpty(x.FirstName));
        
        RuleFor(x=>x.LastName)
            .Cascade(CascadeMode.Stop)
            .MinimumLength(3)
            .WithMessage("نام خانوادگی حداقل باید 3 کارکتر باشد.")
            .MaximumLength(100)
            .WithMessage("نام خانوادگی نمی‌تواند بیشتر از 50 کاراکتر باشد.")
            .When(x=>!string.IsNullOrEmpty(x.LastName));
        
        RuleFor(x => x.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .Matches(@"^09\d{9}$")
            .WithMessage("فرمت شماره تلفن معتبر نیست (مثال: 09123456789).")
            .When(x=>!string.IsNullOrEmpty(x.PhoneNumber));
    }   
}