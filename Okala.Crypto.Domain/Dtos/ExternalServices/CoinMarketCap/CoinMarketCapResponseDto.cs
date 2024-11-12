using System.Text.Json.Serialization;

namespace Okala.Crypto.Domain.Dtos.ExternalServices.CoinMarketCap;

public record CoinMarketCapResponseDto
{
    [JsonPropertyName("status")]
    public StatusDto Status { get; set; }

    [JsonPropertyName("data")]
    public Dictionary<string, CryptoDataDto> Data { get; set; }
}

public record CryptoDataDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("symbol")]
    public string Symbol { get; set; }

    [JsonPropertyName("slug")]
    public string Slug { get; set; }

    [JsonPropertyName("quote")]
    public Dictionary<string, CurrencyQuoteDto> Quote { get; set; }
}

public record CurrencyQuoteDto
{
    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("volume_24h")]
    public decimal Volume24h { get; set; }

    [JsonPropertyName("percent_change_1h")]
    public decimal PercentChange1h { get; set; }

    [JsonPropertyName("percent_change_24h")]
    public decimal PercentChange24h { get; set; }

    [JsonPropertyName("percent_change_7d")]
    public decimal PercentChange7d { get; set; }

    [JsonPropertyName("market_cap")]
    public decimal MarketCap { get; set; }

    [JsonPropertyName("last_updated")]
    public DateTime LastUpdated { get; set; }
}

public record StatusDto
{
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("error_code")]
    public int ErrorCode { get; set; }

    [JsonPropertyName("error_message")]
    public string ErrorMessage { get; set; }
}
