using System;
using System.Text.Json.Serialization;

namespace Okala.Crypto.Domain.Dtos.ExternalServices.CoinMarketCap;

public record CoinMarketCapErrorDto
{
    [JsonPropertyName("status")]
    public ErrorStatusDto Status { get; set; }
}

public record ErrorStatusDto
{
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("error_code")]
    public int ErrorCode { get; set; }

    [JsonPropertyName("error_message")]
    public string ErrorMessage { get; set; }

    [JsonPropertyName("elapsed")]
    public int Elapsed { get; set; }

    [JsonPropertyName("credit_count")]
    public int CreditCount { get; set; }
}

