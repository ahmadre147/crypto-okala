using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Okala.Crypto.Domain.Dtos;
using Okala.Crypto.Domain.Dtos.ExternalServices.CoinMarketCap;
using Okala.Crypto.Domain.Dtos.Quota;
using Okala.Crypto.Domain.Services;

namespace Okala.Crypto.Services.ExternalServices.CoinMarketCap;

/// <summary>
/// Service for interacting with the CoinMarketCap API to retrieve cryptocurrency exchange rates.
/// </summary>
internal class CoinMarketCapHttpClient : ServiceBase, IExternalExchangeService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CoinMarketCapHttpClient> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CoinMarketCapHttpClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client used for API requests.</param>
    /// <param name="options">The application settings containing the API access key.</param>
    /// <param name="logger">The logger used for logging information and errors.</param>
    public CoinMarketCapHttpClient(HttpClient httpClient, IOptions<AppSettings> options, ILogger<CoinMarketCapHttpClient> logger)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://pro-api.coinmarketcap.com");
        _httpClient.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", options.Value.CoinMarketCap.ApiKey);
        _httpClient.DefaultRequestHeaders.Add("Accepts", "application/json");

        _logger = logger;
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
            const string route = "/v1/cryptocurrency/quotes/latest";
            var query = $"?symbol={cryptoCode}&convert={convertOption}";
            var response = await _httpClient.GetAsync(route + query);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadFromJsonAsync<CoinMarketCapErrorDto>();
                _logger.LogWarning("Failed to fetch listings: {0}", errorContent?.Status.ErrorMessage ?? await response.Content.ReadAsStringAsync());
                return null;
            }

            var listings = await response.Content.ReadFromJsonAsync<CoinMarketCapResponseDto>();

            if (listings == null)
            {
                _logger.LogWarning("Failed to deserialize response.");
                return null;
            }

            _logger.LogInformation("Successfully fetched latest listings.");
            return new PricePairDto
            {
                Currency = convertOption,
                Value = listings.Data.FirstOrDefault().Value.Quote[convertOption].Price
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching the latest cryptocurrency listings.");
            return null;
        }
    }
}