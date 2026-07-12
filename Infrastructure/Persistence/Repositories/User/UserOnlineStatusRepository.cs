using Application.Features.User.Interface;
using StackExchange.Redis;

namespace Infrastructure.Persistence.Repositories.User;

public class UserOnlineStatusRepository : UserOnlineStatusRepositoryContract
{
    private readonly IDatabase _redisDatabase;

    private const string ONLINE_USERS_KEY = "online_users";
    private const string USER_CONNECTIONS_PREFIX = "user_connections:";
    private const string CONNECTION_USER_PREFIX = "connection_user:";

    public UserOnlineStatusRepository(IConnectionMultiplexer connection)
    {
        _redisDatabase = connection.GetDatabase();
    }

    public async Task<string?> GetUserIdByConnectionIdAsync(string connectionId)
    {
        return await _redisDatabase
            .StringGetAsync($"{CONNECTION_USER_PREFIX}{connectionId}");
    }

    public async Task RemoveConnectionAsync(string connectionId, string userId)
    {
        await _redisDatabase.SetRemoveAsync( $"{USER_CONNECTIONS_PREFIX}{userId}", connectionId);
    }

    public async Task RemoveConnectionMappingAsync(string connectionId)
    {
        await _redisDatabase.KeyDeleteAsync($"{CONNECTION_USER_PREFIX}{connectionId}");
    }

    public async Task<long> GetConnectionCountAsync(string userId)
    {
        return await _redisDatabase.SetLengthAsync($"{USER_CONNECTIONS_PREFIX}{userId}");
    }

    public async Task RemoveUserFormOnlineListAsync(string userId)
    {
        await _redisDatabase.SetRemoveAsync(ONLINE_USERS_KEY, userId);
    }

    public async Task DeleteUserConnectionsAsync(string userId)
    {
        await _redisDatabase.KeyDeleteAsync($"{USER_CONNECTIONS_PREFIX}{userId}");
    }

    public async Task AddUserConnectionAsync(string userId, string connectionId)
    {
        await _redisDatabase.SetAddAsync(ONLINE_USERS_KEY, userId);
        await _redisDatabase.SetAddAsync($"{USER_CONNECTIONS_PREFIX}{userId}", connectionId);
        await _redisDatabase.StringSetAsync($"{CONNECTION_USER_PREFIX}{connectionId}", userId);
    }

    public async Task MarkUserOnlineAsync(Guid userId, string connectionId)
    {
        try
        {
            await _redisDatabase.SetAddAsync(ONLINE_USERS_KEY, userId.ToString());

            await _redisDatabase.SetAddAsync($"{USER_CONNECTIONS_PREFIX}{userId}", connectionId);

            await _redisDatabase.SetAddAsync($"{CONNECTION_USER_PREFIX}{connectionId}", userId.ToString());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async Task<bool> IsUserOnlineAsync(Guid userId)
    {
        return await _redisDatabase.SetContainsAsync(ONLINE_USERS_KEY, userId.ToString());
    }

    public async Task<List<string>> GetOnlineUsersAsync()
    {
        return (await _redisDatabase
                .SetMembersAsync(ONLINE_USERS_KEY))
            .Select(u => u.ToString())
            .ToList();
    }

    public async Task<List<string>> GetUserConnectionsAsync(string userId)
    {
        var connections = await _redisDatabase
            .SetMembersAsync($"{USER_CONNECTIONS_PREFIX}{userId}");

        return connections.Select(u => u.ToString())
            .ToList();
    }
}