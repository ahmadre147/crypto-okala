using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Okala.Crypto.Domain.Dtos;
using Okala.Crypto.Domain.Dtos.ExternalServices.CoinMarketCap;
using Okala.Crypto.Domain.Dtos.Quota;
using Okala.Crypto.Domain.Services;

namespace Okala.Crypto.Services.ExternalServices.CoinMarketCap;

internal class CoinMarketCapHttpClient : ServiceBase, IExternalExchangeService
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

    public async Task<PricePairDto?> GetExchangeRatesAsync(string cryptoCode, string convertOption)
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

            if (listings == null)
            {
                _logger.LogWarning("Failed to fetch deserialize response.");

                return null;
            }
            
            _logger.LogInformation("Successfully fetched latest listings.");
            return new PricePairDto(convertOption, listings!.Data.FirstOrDefault().Value.Quote[convertOption].Price);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching the latest cryptocurrency listings.");
            return null;
        }
    }

}
