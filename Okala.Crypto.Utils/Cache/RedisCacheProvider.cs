using MessagePack;
using StackExchange.Redis;

namespace Okala.Crypto.Utils.Cache;

public class RedisCacheProvider(IConnectionMultiplexer redis, TimeSpan commandTimeout) : ICacheProvider
{
    private IDatabase Database => redis.GetDatabase();

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var data = await Database.StringGetAsync(key).WaitAsync(commandTimeout, cancellationToken);
        return data.HasValue
            ? MessagePackSerializer.Deserialize<T>(data, cancellationToken: cancellationToken)
            : default;
    }
    
    public T? Get<T>(string key)
    {
        var data = Database.StringGet(key);
        return data.HasValue
            ? MessagePackSerializer.Deserialize<T>(data)
            : default;
    }
    
    public async Task SetAsync<T>(string key, T value, TimeSpan expiry, CancellationToken cancellationToken = default)
    {
        var data = MessagePackSerializer.Serialize(value, cancellationToken: cancellationToken);
        await Database.StringSetAsync(key, data, expiry).WaitAsync(commandTimeout, cancellationToken);
    }
    
    public void Set<T>(string key, T value, TimeSpan expiry)
    {
        var data = MessagePackSerializer.Serialize(value);
        Database.StringSet(key, data, expiry);
    }
    
    public async Task DeleteAsync(string key, CancellationToken cancellationToken = default) =>
        await Database.KeyDeleteAsync(key).WaitAsync(commandTimeout, cancellationToken);
    
    public void Delete(string key) => Database.KeyDelete(key);
}