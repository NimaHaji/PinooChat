namespace Application.Features.ChatMessages.DTOs;

public class SendMessageDto
{
    public Guid SenderId { get;  set; }
    public Guid ReceiverId { get;  set; } 
    public string Content { get; set; } = null!;
}