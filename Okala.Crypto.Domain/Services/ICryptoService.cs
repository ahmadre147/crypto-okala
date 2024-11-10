using Okala.Crypto.Domain.Dtos;

namespace Okala.Crypto.Domain.Services;

public interface ICryptoService
{
    Task<ServiceResult<double>> GetQuotaAsync(string symbol);
}