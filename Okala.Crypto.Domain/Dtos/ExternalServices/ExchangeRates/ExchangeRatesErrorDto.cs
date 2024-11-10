using System.Text.Json.Serialization;

namespace Okala.Crypto.Domain.Dtos.ExternalServices.ExchangeRates;

public record ExchangeRatesErrorDto
{
    [JsonPropertyName("error")]
    public ErrorDetail Error { get; set; }
}

public record ErrorDetail
{
    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }
}
