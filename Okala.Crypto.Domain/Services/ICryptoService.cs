using Okala.Crypto.Domain.Dtos;
using Okala.Crypto.Domain.Dtos.Quota;

namespace Okala.Crypto.Domain.Services;

public interface ICryptoService
{
    Task<ServiceResult<QuotaResponseDto?>> GetQuotaAsync(string symbol);
}