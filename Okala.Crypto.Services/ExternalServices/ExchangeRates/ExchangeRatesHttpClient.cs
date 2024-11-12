using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Okala.Crypto.Domain.Dtos;
using Okala.Crypto.Domain.Dtos.ExternalServices.ExchangeRates;
using Okala.Crypto.Domain.Dtos.Quota;
using Okala.Crypto.Domain.Services;

namespace Okala.Crypto.Services.ExternalServices.ExchangeRates;

/// <summary>
/// Service for interacting with the ExchangeRates API to retrieve cryptocurrency exchange rates.
/// </summary>
internal class ExchangeRatesHttpClient : ServiceBase, IExternalExchangeService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ExchangeRatesHttpClient> _logger;
    private readonly string _accessKey;
        
    /// <summary>
    /// Initializes a new instance of the <see cref="ExchangeRatesHttpClient"/> class.
    /// </summary>
    /// <param name="client">The HTTP client used for API requests.</param>
    /// <param name="options">The application settings containing the API access key.</param>
    /// <param name="logger">The logger used for logging information and errors.</param>
    public ExchangeRatesHttpClient(HttpClient client, IOptions<AppSettings> options, ILogger<ExchangeRatesHttpClient> logger)
    {
        _httpClient = client;
        _httpClient.BaseAddress = new Uri("https://api.exchangeratesapi.io/v1");
        _logger = logger;
        _accessKey = options.Value.ExchangeRates.ApiKey;
    }
        
    /// <summary>
    /// Retrieves the exchange rate for a specified cryptocurrency and conversion option.
    /// </summary>
    /// <param name="cryptoCode">The base cryptocurrency code (e.g., BTC).</param>
    /// <param name="convertOption">The target currency to convert to (e.g., USD).</param>
    /// <returns>A <see cref="PricePairDto"/> containing the currency and its converted value, or null if the request fails.</returns>
    public async Task<PricePairDto?> GetExchangeRatesAsync(string cryptoCode, string convertOption)
    {
        try
        {
            var query = $"?access_key={_accessKey}&base={cryptoCode}&symbols={convertOption}&format=1";
            var response = await _httpClient.GetAsync(query);
                
            if (!response.IsSuccessStatusCode)
            {
                var apiError = await response.Content.ReadFromJsonAsync<ExchangeRatesErrorDto>();
                _logger.LogWarning("Failed to fetch exchange rates. Status code: {0}, Message: {1}", response.StatusCode, apiError?.Error.Message);
                return null;
            }

            var exchangeRates = await response.Content.ReadFromJsonAsync<ExchangeRatesResponseDto>();
                
            if (exchangeRates == null)
            {
                _logger.LogWarning("Failed to deserialize response.");
                return null;
            }

            if (!exchangeRates.Success)
            {
                throw new Exception("Failed to fetch exchange rates from the API.");
            }
                
            _logger.LogInformation("Successfully fetched exchange rates for {0}.", cryptoCode);

            return new PricePairDto
            {
                Currency = convertOption,
                Value = exchangeRates.Rates[convertOption]
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching exchange rates.");
            return null;
        }
    }
}