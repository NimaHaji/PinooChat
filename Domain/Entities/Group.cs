namespace Domain.Entities;

public class Group
{

    public Guid ConversationId { get; private set; }
    
    public string GroupIdName { get; private set; }
    
    public string GroupTitle { get; private set; }
    
    public string? Description { get; private set; }
    
    public Guid OwnerId { get; private set; }
    
    public Conversation Conversation { get; private set; } = null!;

    private Group()
    {
    }

    public Group(
        Guid conversationId,
        string groupName,
        string groupTitle,
        Guid ownerId,
        string? description = null)
    {
        ConversationId = conversationId;
        GroupIdName = groupName;
        OwnerId = ownerId;
        Description = description;
        GroupTitle = groupTitle;
    }
    
    public void Rename(string name)
    {
        GroupIdName = name;
    }

    public void ChangeDescription(string? description)
    {
        Description = description;
    }
}