using Application.Common.Interfaces;
using Infrastructure.Persistence.Contexts;

namespace Infrastructure.Persistence;

public class UnitOfWork:UnitOfWorkContract
{
    private readonly ChatContext _chatContext;

    public UnitOfWork(ChatContext chatContext)
    {
        _chatContext = chatContext;
    }

    public async Task SaveAsync()
    {
        await _chatContext.SaveChangesAsync();
    }
}