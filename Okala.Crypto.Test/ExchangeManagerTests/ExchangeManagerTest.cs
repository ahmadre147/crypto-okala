using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Okala.Crypto.Domain.Dtos.Quota;
using Okala.Crypto.Domain.Services;

namespace Okala.Crypto.Test.ExchangeManagerTests;

public class ExchangeManagerTests
{
    private readonly Mock<IExternalExchangeService> _exchangeServiceMock;
    private readonly IExchangeManager _exchangeManager;

    public ExchangeManagerTests()
    {
        _exchangeServiceMock = new Mock<IExternalExchangeService>();
        var logger = new NullLogger<ExchangeManager>();
        _exchangeManager = new ExchangeManager(new[] { _exchangeServiceMock.Object }, logger);
    }

    [Fact]
    public async Task GetQuotaAsync_FetchesDataFromExchangeService()
    {
        // Arrange
        var symbol = "BTC";
        var pricePairs = new List<PricePairDto>
        {
            new PricePairDto { Currency = "USD", Value = 30000 },
            new PricePairDto { Currency = "EUR", Value = 27000 }
        };

        _exchangeServiceMock.Setup(es => es.GetExchangeRatesAsync(symbol, It.IsAny<string>()))
            .ReturnsAsync((string symbol, string convertOption) => 
                pricePairs.Find(p => p.Currency == convertOption));

        // Act
        var result = await _exchangeManager.GetQuotaAsync(symbol);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Prices.Count);
    }

    [Fact]
    public async Task GetQuotaAsync_NoData_ReturnsNull()
    {
        // Arrange
        var symbol = "BTC";
        _exchangeServiceMock.Setup(es => es.GetExchangeRatesAsync(symbol, It.IsAny<string>()))
            .ReturnsAsync((PricePairDto?)null);

        // Act
        var result = await _exchangeManager.GetQuotaAsync(symbol);

        // Assert
        Assert.Null(result);
    }
}
