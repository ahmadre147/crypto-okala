using Microsoft.Extensions.Logging;
using Okala.Crypto.Domain.Dtos;
using Okala.Crypto.Domain.Dtos.Quota;
using Okala.Crypto.Domain.Enumeration;
using Okala.Crypto.Domain.Services;

namespace Okala.Crypto.Services;

/// <summary>
/// Manages the retrieval of cryptocurrency quota information from various external exchange services.
/// </summary>
internal class ExchangeManager : ServiceBase, IExchangeManager
{
    private readonly List<IExternalExchangeService> _exchangeServices;
    private readonly ILogger<ExchangeManager> _logger;
    private readonly List<string> _convertOptions = ["USD", "EUR", "BRL", "GBP", "AUD"];

    /// <summary>
    /// Initializes a new instance of the <see cref="ExchangeManager"/> class.
    /// </summary>
    /// <param name="exchangeServices">The collection of external exchange services to retrieve data from.</param>
    /// <param name="logger">The logger for logging information and errors.</param>
    public ExchangeManager(IEnumerable<IExternalExchangeService> exchangeServices, ILogger<ExchangeManager> logger)
    {
        _exchangeServices = exchangeServices.ToList();
        _logger = logger;
    }

    /// <summary>
    /// Retrieves quota information for a specified cryptocurrency symbol from available exchange services.
    /// </summary>
    /// <param name="symbol">The cryptocurrency symbol (e.g., BTC) to retrieve quota information for.</param>
    /// <param name="cancellationToken">Optional token for canceling the asynchronous operation.</param>
    /// <returns>A <see cref="QuotaResponseDto"/> containing pricing information in multiple currencies, or null if no data is available.</returns>
    public async Task<ServiceResult<QuotaResponseDto>> GetQuotaAsync(string symbol, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting quota retrieval for symbol '{Symbol}'.", symbol);

        foreach (var service in _exchangeServices)
        {
            _logger.LogInformation("Attempting to retrieve quota data from service: {ServiceName}", service.GetType().Name);

            var pairs = new List<PricePairDto>();
            foreach (var convertOption in _convertOptions)
            {
                var value = await service.GetExchangeRatesAsync(symbol, convertOption);
                    
                if (value != null)
                {
                    _logger.LogInformation("Retrieved price for {Symbol} in {Currency} from {ServiceName}: {Price}",
                        symbol, convertOption, service.GetType().Name, value.Value);
                    pairs.Add(value);
                }
                else
                {
                    _logger.LogWarning("No value found for {Symbol} in {Currency} from {ServiceName}. Aborting further requests to this service.",
                        symbol, convertOption, service.GetType().Name);
                    break;
                }
            }

            if (pairs.Count > 0)
            {
                _logger.LogInformation("Successfully retrieved quota for symbol '{Symbol}' from service '{ServiceName}'.",
                    symbol, service.GetType().Name);
                return SuccessResult(new QuotaResponseDto
                {
                    Prices = pairs
                });
            }
        }
            
        _logger.LogWarning("Quota retrieval failed for symbol '{Symbol}'. No data found from available services.", symbol);
        return ErrorResult<QuotaResponseDto>(ErrorType.NotFound);
    }
}