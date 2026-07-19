using Application.Features.ChatMessages.DTOs;
using FluentValidation;

namespace Application.Features.User.Validators;

public class SendMessageValidator:AbstractValidator<SendMessageDto>
{
    public SendMessageValidator()
    {
        RuleFor(x => x.SenderId)
            .NotEmpty()
            .WithMessage("گیرنده پیام نمیتواند خالی باشد");

        RuleFor(x => x.ReceiverId)
            .NotEmpty()
            .WithMessage("فرستنده پیام نمیتواند خالی باشد");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("پیام نمیتواند خالی باشد")
            .MaximumLength(1000)
            .WithMessage("حداکثر تعداد کرکتر پیام 1000 است");
    }
}