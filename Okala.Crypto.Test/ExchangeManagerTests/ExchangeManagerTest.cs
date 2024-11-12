using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Okala.Crypto.Domain.Dtos.Quota;
using Okala.Crypto.Domain.Services;
using Okala.Crypto.Services;
using Okala.Crypto.Services.ExternalServices.CoinMarketCap;

namespace Okala.Crypto.Test.ExchangeManagerTests;

public class ExchangeManagerTests
{
    private readonly ExchangeManager _exchangeManager;
    private readonly Mock<IExternalExchangeService> _coinMarketCapMock;
    private readonly Mock<IExternalExchangeService> _exchangeRatesMock;

    public ExchangeManagerTests()
    {
        _coinMarketCapMock = new Mock<IExternalExchangeService>();
        _exchangeRatesMock = new Mock<IExternalExchangeService>();
        var logger = new NullLogger<ExchangeManager>();
        
        _exchangeManager = new ExchangeManager([_coinMarketCapMock.Object, _exchangeRatesMock.Object], logger);
    }

    [Fact]
    public async Task GetQuotaAsync_FetchesDataFromExchangeService()
    {
        var symbol = "BTC";
        List<PricePairDto> pricePairs =
        [
            new PricePairDto { Currency = "USD", Value = 26000 },
            new PricePairDto { Currency = "EUR", Value = 27000 },
            new PricePairDto { Currency = "BRL", Value = 28000 },
            new PricePairDto { Currency = "GBP", Value = 29000 },
            new PricePairDto { Currency = "AUD", Value = 30000 },
        ];

        _coinMarketCapMock.Setup(es => es.GetExchangeRatesAsync(symbol, It.IsAny<string>()))
            .ReturnsAsync((string _, string convertOption) => 
                pricePairs.Find(p => p.Currency == convertOption));

        var result = await _exchangeManager.GetQuotaAsync(symbol);

        Assert.NotNull(result);
        Assert.Equal(5, result.Data.Prices.Count);
    }

    [Fact]
    public async Task GetQuotaAsync_NoData_ReturnsNull()
    {
        var symbol = "DDD";
        _coinMarketCapMock.Setup(es => es.GetExchangeRatesAsync(symbol, It.IsAny<string>()))
            .ReturnsAsync((PricePairDto?)null);

        var result = await _exchangeManager.GetQuotaAsync(symbol);

        Assert.False(result.Success);
    }
}
