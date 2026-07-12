public class SentChatMessageDto
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; }
    public Guid ReceiverId { get; set; }
    public string ReceiverName { get; set; }
    public string Content { get; set; } = null!;
    public DateTimeOffset TimeStamp { get; set; }
}