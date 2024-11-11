namespace Okala.Crypto.Utils.Cache;

/// <summary>
/// Cache provider interface. Implementations must provide serde functionality with async.
/// </summary>
public interface ICacheProvider
{
    /// <summary>
    /// Gets the item and deserialize it 
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T">Type of the item value</typeparam>
    /// <returns>Deserialized value of the cached item</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the item and deserialize it 
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <typeparam name="T">Type of the item value</typeparam>
    /// <returns>Deserialized value of the cached item</returns>
    T? Get<T>(string key);
    
    /// <summary>
    /// Serializes and stores item in cache.
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="value">Value of the item to be caches</param>
    /// <param name="expiry">Cache expiration time</param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T">Type of the item value</typeparam>
    /// <returns></returns>
    Task SetAsync<T>(string key, T value, TimeSpan expiry, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Serializes and stores item in cache.
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="value">Value of the item to be caches</param>
    /// <param name="expiry">Cache expiration time</param>
    /// <typeparam name="T">Type of the item value</typeparam>
    /// <returns></returns>
    void Set<T>(string key, T value, TimeSpan expiry);
    
    /// <summary>
    /// Deletes item from cache. (Invalidating cache item)
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteAsync(string key, CancellationToken cancellationToken);
    
    /// <summary>
    /// Deletes item from cache. (Invalidating cache item)
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <returns></returns>
    void Delete(string key);
}