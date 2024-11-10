namespace Okala.Crypto.Utils.Cache;


public interface ICacheLayer<T>
{
    Task<T?> GetValueAsync(int id, CancellationToken cancellationToken = default);
    T? GetValue(int id);
    Task SetAsync(int id, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
    void Set(int id, T value, TimeSpan? expiry = null);
    Task RemoveAsync(int id, CancellationToken cancellationToken = default);
    void Remove(int id);
}
