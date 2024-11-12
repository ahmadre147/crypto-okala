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
internal class CryptoService(
    ICacheProvider cacheProvider,
    IExchangeManager exchangeManager,
    ILogger<CryptoService> logger)
    : ServiceBase, ICryptoService
{
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
            logger.LogInformation("Attempting to retrieve quota information for symbol '{Symbol}' from cache.", symbol);

            var quota = await cacheProvider.GetAsync<QuotaResponseDto>(CacheKey(symbol));

            if (quota == null)
            {
                logger.LogInformation("Cache miss for symbol '{Symbol}'. Attempting to retrieve from exchange services.", symbol);

                var externalResponse = await exchangeManager.GetQuotaAsync(symbol);

                if (externalResponse.Success)
                    quota = externalResponse.Data;
                else
                    return externalResponse;

                await cacheProvider.SetAsync(CacheKey(symbol), quota, TimeSpan.FromSeconds(1));
                logger.LogInformation("Cached quota information for symbol '{Symbol}' with a 1-second expiration.", symbol);
            }
            else
            {
                logger.LogInformation("Cache hit for symbol '{Symbol}'. Returning cached quota information.", symbol);
            }

            return SuccessResult(quota);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while retrieving quota information for symbol '{Symbol}'.", symbol);
            return ErrorResult<QuotaResponseDto>(ErrorType.UnknownError);
        }
    }
}