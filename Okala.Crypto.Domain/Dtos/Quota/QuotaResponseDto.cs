namespace Okala.Crypto.Domain.Dtos.Quota;

public class QuotaResponseDto(List<PricePairDto> Prices);

public record PricePairDto(string Currency, decimal Value);