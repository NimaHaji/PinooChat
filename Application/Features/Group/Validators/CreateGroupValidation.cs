using Application.Features.Group.DTOs;
using FluentValidation;

namespace Application.Features.Group.Validators;

public class CreateGroupValidation : AbstractValidator<CreateGroupDto>
{
    public CreateGroupValidation()
    {
        RuleFor(g => g.OwnerId)
            .NotEmpty()
            .WithMessage("سازنده گروه معتبر نیست.");

        RuleFor(g => g.GroupTitle)
            .NotEmpty()
            .WithMessage("نام گروه نمی‌تواند خالی باشد.")
            .MaximumLength(50)
            .WithMessage("نام گروه نمی‌تواند بیشتر از 50 کاراکتر باشد.");

        RuleFor(g => g.GroupIdName)
            .NotEmpty()
            .WithMessage("شناسه گروه الزامی است.")
            .MinimumLength(3)
            .WithMessage("شناسه گروه باید حداقل 3 کاراکتر باشد.")
            .MaximumLength(30)
            .WithMessage("شناسه گروه نمی‌تواند بیشتر از 30 کاراکتر باشد.")
            .Matches("^[a-zA-Z0-9_]+$")
            .WithMessage("شناسه گروه فقط می‌تواند شامل حروف انگلیسی، اعداد و _ باشد.");

        RuleFor(g => g.Description)
            .MaximumLength(500)
            .WithMessage("توضیحات نمی‌تواند بیشتر از 500 کاراکتر باشد.");
    }
}