using Okala.Crypto.Domain.Dtos;
using Okala.Crypto.Domain.Dtos.Quota;

namespace Okala.Crypto.Domain.Services;

public interface IExchangeManager
{ 
    Task<ServiceResult<QuotaResponseDto>> GetQuotaAsync(string symbol, CancellationToken cancellationToken = default);
}