using Microsoft.Extensions.Logging;
using Okala.Crypto.Domain.Dtos;
using Okala.Crypto.Domain.Dtos.Quota;
using Okala.Crypto.Domain.Services;
using Okala.Crypto.Utils.Cache;

namespace Okala.Crypto.Services;

internal class CryptoService(ICacheProvider cacheProvider, IExchangeManager exchangeManager, ILogger<CryptoService> logger) : ServiceBase, ICryptoService
{
    private static string CacheKey(string symbol) => $"quota:{symbol}";

    public async Task<ServiceResult<QuotaResponseDto?>> GetQuotaAsync(string symbol)
    {
        try
        {
            var quota = await cacheProvider.GetAsync<QuotaResponseDto>(CacheKey(symbol));

            if (quota == null)
            {
                quota = await exchangeManager.GetQuotaAsync(symbol);

                await cacheProvider.SetAsync(CacheKey(symbol), quota, TimeSpan.FromSeconds(1));
            }

            return SuccessResult(quota);
        }
        catch(Exception e)
        {
            logger.LogError(e, "Error");

            return ErrorResult<QuotaResponseDto>(null);
        }
    }
}