namespace Okala.Crypto.Domain.Dtos;

public class AppSettings
{
    public Redis Redis { get; set; }
    public ExchangeRates ExchangeRates { get; set; }
    public CoinMarketCap CoinMarketCap { get; set; }
}

public class Redis
{
    public string Url { get; set; }
    public int DefaultTTL { get; set; }
}

public class ExchangeRates
{
    public string ApiKey { get; set; }
}

public class CoinMarketCap
{
    public string ApiKey { get; set; }
}
