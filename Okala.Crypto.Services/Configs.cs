using Microsoft.Extensions.DependencyInjection;
using Okala.Crypto.Services.ExternalServices.CoinMarketCap;
using Okala.Crypto.Services.ExternalServices.ExchangeRates;

namespace Okala.Crypto.Services;

public static class Configs
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddHttpClient<CoinMarketCapHttpClient>();
        services.AddHttpClient<ExchangeRatesHttpClient>();
    }
}