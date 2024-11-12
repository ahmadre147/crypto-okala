using Microsoft.Extensions.Logging;
using Okala.Crypto.Domain.Dtos;
using Okala.Crypto.Domain.Dtos.Quota;
using Okala.Crypto.Domain.Enumeration;
using Okala.Crypto.Domain.Services;
using Okala.Crypto.Utils.Cache;

namespace Okala.Crypto.Services;

/// <summary>
/// Service for retrieving cryptocurrency quota information, with caching to improve performance.
/// </summary>
internal class CryptoService : ServiceBase, ICryptoService
{
    private readonly ICacheProvider _cacheProvider;
    private readonly IExchangeManager _exchangeManager;
    private readonly ILogger<CryptoService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CryptoService"/> class.
    /// </summary>
    /// <param name="cacheProvider">The cache provider for caching quota information.</param>
    /// <param name="exchangeManager">The manager for accessing exchange quota data.</param>
    /// <param name="logger">The logger for logging errors and information.</param>
    public CryptoService(ICacheProvider cacheProvider, IExchangeManager exchangeManager, ILogger<CryptoService> logger)
    {
        _cacheProvider = cacheProvider;
        _exchangeManager = exchangeManager;
        _logger = logger;
    }

    private static string CacheKey(string symbol) => $"quota:{symbol}";

    /// <summary>
    /// Retrieves the quota information for the specified cryptocurrency symbol, using caching to optimize retrieval.
    /// </summary>
    /// <param name="symbol">The cryptocurrency symbol (e.g., BTC) to retrieve quota information for.</param>
    /// <returns>A <see cref="ServiceResult{QuotaResponseDto}"/> containing the quota information, or an error if the retrieval fails.</returns>
    public async Task<ServiceResult<QuotaResponseDto>> GetQuotaAsync(string symbol)
    {
        try
        {
            _logger.LogInformation("Attempting to retrieve quota information for symbol '{Symbol}' from cache.", symbol);

            var quota = await _cacheProvider.GetAsync<QuotaResponseDto>(CacheKey(symbol));

            if (quota == null)
            {
                _logger.LogInformation("Cache miss for symbol '{Symbol}'. Attempting to retrieve from exchange services.", symbol);

                var externalResponse = await _exchangeManager.GetQuotaAsync(symbol);

                if (externalResponse.Success)
                    quota = externalResponse.Data;
                else
                    return externalResponse;

                await _cacheProvider.SetAsync(CacheKey(symbol), quota, TimeSpan.FromSeconds(1));
                _logger.LogInformation("Cached quota information for symbol '{Symbol}' with a 1-second expiration.", symbol);
            }
            else
            {
                _logger.LogInformation("Cache hit for symbol '{Symbol}'. Returning cached quota information.", symbol);
            }

            return SuccessResult(quota);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while retrieving quota information for symbol '{Symbol}'.", symbol);
            return ErrorResult<QuotaResponseDto>(ErrorType.UnknownError);
        }
    }
}