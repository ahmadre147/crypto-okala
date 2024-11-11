using Microsoft.Extensions.Logging;
using Okala.Crypto.Domain.Dtos.Quota;
using Okala.Crypto.Domain.Services;

namespace Okala.Crypto.Services;

public class ExchangeManager(IEnumerable<IExternalExchangeService> exchangeServices, ILogger<ExchangeManager> logger): IExchangeManager
{
    private readonly List<IExternalExchangeService> _exchangeServices = exchangeServices.ToList();
    private readonly List<string> _convertOptions = ["USD", "EUR", "AUD"];

    public async Task<QuotaResponseDto> GetQuotaAsync(string symbol, CancellationToken cancellationToken = default)
    {
        foreach (var service in _exchangeServices)
        {
            logger.LogInformation("Checking service: {}", service.GetType().Name);

            var pairs = new List<PricePairDto>();
            var value = await service.GetExchangeRatesAsync(symbol, );
            if (value != null)
            {
                logger.LogInformation("Value found in layer: {LayerType} for ID {Id}.", layer.GetType().Name, id);
                return value;
            }
        }
        
        logger.LogWarning("Value not found for symbol {}.", symbol);
        return null;
    }
}