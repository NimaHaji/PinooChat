using Application.Features.Auth.DTOs;
using FluentValidation;

namespace Application.Validator.User;

public class RegisterUserValidator : AbstractValidator<RegisterUserRequestDto>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.UserName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("نام کاربری الزامی است.")
            .MaximumLength(50).WithMessage("نام کاربری نمی‌تواند بیشتر از ۵۰ کاراکتر باشد.")
            .Matches(@"^[a-zA-Z0-9_]+$")
            .WithMessage("نام کاربری فقط می‌تواند شامل حروف انگلیسی، اعداد و زیرخط (_) باشد.");
        
        RuleFor(x => x.FirstName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("نام الزامی است .")
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("نام نباید خالی یا فاصله باشد .")
            .MaximumLength(50)
            .WithMessage("نام نمیتواند بیشتر از 50 کاراکتر باشد");

        RuleFor(x => x.LastName)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(50)
            .WithMessage("نام خانوادگی بیشتر از 50 کاراکتر باشد")
            .When(x => !string.IsNullOrWhiteSpace(x.LastName));

        RuleFor(x => x.MobilePhone)
            .Cascade(CascadeMode.Stop)
            .Matches(@"^09\d{9}$")
            .WithMessage("شماره موبایل باید معتبر باشد .")
            .When(x => !string.IsNullOrWhiteSpace(x.MobilePhone));
        
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("ایمیل الزامی است .")
            .EmailAddress().WithMessage("ایمیل معتیر نیست .")
            .MaximumLength(100);

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("پسورد الزامی است")
            .MinimumLength(8)
            .WithMessage("پسوورد باید حداقل 8 کاراکتر داشته باشد .")
            .Matches("[A-Z]")
            .WithMessage("پسوورد باید حداقل شامل یک حرف بزرگ باشد .")
            .Matches("[a-z]")
            .WithMessage("پسوورد باید حداقل یک حرف کوچک داشته باشد .")
            .Matches("[0-9]")
            .WithMessage("پسوورد باید حداقل یک عدد داشته باشد .");

    }
}