using MessagePack;
using StackExchange.Redis;

namespace Okala.Crypto.Utils.Cache;

public class RedisCacheProvider<T>(IConnectionMultiplexer redis, TimeSpan commandTimeout) : ICacheLayer<T>
{
    private readonly IDatabase _database = redis.GetDatabase();

    public async Task<T?> GetValueAsync(int id, CancellationToken cancellationToken = default)
    {
        var key = id.ToString();
        var data = await _database.StringGetAsync(key).WaitAsync(commandTimeout, cancellationToken);
        return data.HasValue
            ? MessagePackSerializer.Deserialize<T>(data, cancellationToken: cancellationToken)
            : default;
    }

    public T? GetValue(int id)
    {
        var key = id.ToString();
        var data = _database.StringGet(key);
        return data.HasValue
            ? MessagePackSerializer.Deserialize<T>(data)
            : default;
    }

    public async Task SetAsync(int id, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        var key = id.ToString();
        var data = MessagePackSerializer.Serialize(value, cancellationToken: cancellationToken);
        await _database.StringSetAsync(key, data, expiry).WaitAsync(commandTimeout, cancellationToken);
    }

    public void Set(int id, T value, TimeSpan? expiry = null)
    {
        var key = id.ToString();
        var data = MessagePackSerializer.Serialize(value);
        _database.StringSet(key, data, expiry);
    }

    public async Task RemoveAsync(int id, CancellationToken cancellationToken = default)
    {
        var key = id.ToString();
        await _database.KeyDeleteAsync(key).WaitAsync(commandTimeout, cancellationToken);
    }

    public void Remove(int id)
    {
        var key = id.ToString();
        _database.KeyDelete(key);
    }
}