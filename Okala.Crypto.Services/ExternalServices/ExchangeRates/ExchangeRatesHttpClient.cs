using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Okala.Crypto.Domain.Dtos;
using Okala.Crypto.Domain.Dtos.ExternalServices.ExchangeRates;
using Okala.Crypto.Domain.Dtos.Quota;
using Okala.Crypto.Domain.Services;

namespace Okala.Crypto.Services.ExternalServices.ExchangeRates;

internal class ExchangeRatesHttpClient : ServiceBase, IExternalExchangeService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ExchangeRatesHttpClient> _logger;
    
    public ExchangeRatesHttpClient(HttpClient client, IOptions<AppSettings> options, ILogger<ExchangeRatesHttpClient> logger)
    {
        _httpClient = client;
        _httpClient.BaseAddress = new Uri($"https://api.exchangeratesapi.io/v1/latest?access_key={options.Value.ExchangeRates.ApiKey}");
        _logger = logger;
    }
    
    public async Task<PricePairDto?> GetExchangeRatesAsync(string cryptoCode, string convertOption)
    {
        try
        {
            var query = $"&base={cryptoCode}&symbols=USD,EUR,BRL,GBP,AUD&format=1";

            var response = await _httpClient.GetAsync(query);
            
            if (!response.IsSuccessStatusCode)
            {
                var apiError = await response.Content.ReadFromJsonAsync<ExchangeRatesErrorDto>();
                
                _logger.LogWarning("Failed to fetch exchange rates. Status code: {}, Message: {}", response.StatusCode, apiError?.Error.Message);
                return null;
            }

            var exchangeRates = await response.Content.ReadFromJsonAsync<ExchangeRatesResponseDto>();
            
            if (exchangeRates == null)
            {
                _logger.LogWarning("Failed to fetch deserialize response.");

                return null;
            }
            
            _logger.LogInformation("Successfully fetched exchange rates for {}.", cryptoCode);
            return new PricePairDto(convertOption, exchangeRates.Rates[convertOption]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching exchange rates.");
            return null;
        }
    }
}
