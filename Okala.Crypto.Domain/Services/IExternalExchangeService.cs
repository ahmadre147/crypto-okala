using Okala.Crypto.Domain.Dtos.Quota;

namespace Okala.Crypto.Domain.Services;

public interface IExternalExchangeService
{
    Task<PricePairDto?> GetExchangeRatesAsync(string cryptoCode, string convertOption);
}