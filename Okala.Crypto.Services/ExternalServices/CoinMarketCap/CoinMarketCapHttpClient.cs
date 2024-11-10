using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Okala.Crypto.Domain.Dtos;
using Okala.Crypto.Domain.Dtos.ExternalServices.CoinMarketCap;

namespace Okala.Crypto.Services.ExternalServices.CoinMarketCap;

internal class CoinMarketCapHttpClient : ServiceBase
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CoinMarketCapHttpClient> _logger;

    public CoinMarketCapHttpClient(HttpClient httpClient, IOptions<AppSettings> options, ILogger<CoinMarketCapHttpClient> logger)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://pro-api.coinmarketcap.com/v1/cryptocurrency/listings/latest");
        _httpClient.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", options.Value.CoinMarketCap.ApiKey);
        _httpClient.DefaultRequestHeaders.Add("Accepts", "application/json");

        _logger = logger;
    }

    /// <summary>
    /// Fetches exchange rates for the given cryptocurrency against specified currencies.
    /// </summary>
    /// <param name="cryptoCode">The cryptocurrency code (e.g., BTC).</param>
    /// <param name="convertOption">The convert option (e.g., USD, EUR).</param>
    /// <returns>A DTO with exchange rates for the cryptocurrency.</returns>
    public async Task<CoinMarketCapResponseDto?> GetExchangeRatesAsync(string cryptoCode, string convertOption)
    {
        try
        {
            var query = $"?symbol={cryptoCode}&convert={convertOption}";
            var response = await _httpClient.GetAsync(query);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to fetch listings: {}", errorContent);
                return null;
            }

            var listings = await response.Content.ReadFromJsonAsync<CoinMarketCapResponseDto>();
            _logger.LogInformation("Successfully fetched latest listings.");
            return listings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching the latest cryptocurrency listings.");
            return null;
        }
    }

}
