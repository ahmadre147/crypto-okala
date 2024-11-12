using Microsoft.Extensions.DependencyInjection;
using Okala.Crypto.Domain.Services;
using Okala.Crypto.Services.ExternalServices.CoinMarketCap;
using Okala.Crypto.Services.ExternalServices.ExchangeRates;
using Okala.Crypto.Utils.Cache;

namespace Okala.Crypto.Services;

public static class Configs
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddHttpClient<IExternalExchangeService, ExchangeRatesHttpClient>();
        services.AddHttpClient<IExternalExchangeService, CoinMarketCapHttpClient>();

        services
            .AddScoped<ICryptoService, CryptoService>()
            .AddScoped<IExchangeManager, ExchangeManager>();
    }
}