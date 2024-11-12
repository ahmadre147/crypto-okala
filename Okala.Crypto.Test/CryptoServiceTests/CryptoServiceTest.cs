using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Okala.Crypto.Domain.Dtos.Quota;
using Okala.Crypto.Domain.Enumeration;
using Okala.Crypto.Domain.Services;
using Okala.Crypto.Utils.Cache;

namespace Okala.Crypto.Test.CryptoServiceTests;

public class CryptoServiceTest
{
    private readonly Mock<ICacheProvider> _cacheProviderMock;
    private readonly Mock<IExchangeManager> _exchangeManagerMock;
    private readonly ICryptoService _cryptoService;

    public CryptoServiceTests()
    {
        _cacheProviderMock = new Mock<ICacheProvider>();
        _exchangeManagerMock = new Mock<IExchangeManager>();
        var logger = new NullLogger<ICryptoService>();
        _cryptoService = new CryptoService(_cacheProviderMock.Object, _exchangeManagerMock.Object, logger);
    }

    [Fact]
    public async Task GetQuotaAsync_CacheHit_ReturnsCachedData()
    {
        var symbol = "BTC";
        var quotaData = new QuotaResponseDto { /* fill with test data */ };
        _cacheProviderMock.Setup(cp => cp.GetAsync<QuotaResponseDto>(It.IsAny<string>()))
                          .ReturnsAsync(quotaData);

        // Act
        var result = await _cryptoService.GetQuotaAsync(symbol);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(quotaData, result.Data);
        _cacheProviderMock.Verify(cp => cp.GetAsync<QuotaResponseDto>(It.Is<string>(s => s.Contains(symbol))), Times.Once);
        _exchangeManagerMock.Verify(em => em.GetQuotaAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetQuotaAsync_CacheMiss_FetchesFromExchangeAndCachesData()
    {
        // Arrange
        var symbol = "BTC";
        var quotaData = new QuotaResponseDto { /* fill with test data */ };
        _cacheProviderMock.Setup(cp => cp.GetAsync<QuotaResponseDto>(It.IsAny<string>()))
                          .ReturnsAsync((QuotaResponseDto?)null);
        _exchangeManagerMock.Setup(em => em.GetQuotaAsync(symbol, default))
                            .ReturnsAsync(quotaData);

        // Act
        var result = await _cryptoService.GetQuotaAsync(symbol);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(quotaData, result.Data);
        _cacheProviderMock.Verify(cp => cp.SetAsync(It.IsAny<string>(), quotaData, It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task GetQuotaAsync_Exception_ReturnsErrorResult()
    {
        // Arrange
        var symbol = "BTC";
        _cacheProviderMock.Setup(cp => cp.GetAsync<QuotaResponseDto>(It.IsAny<string>()))
                          .Throws(new Exception("Cache error"));

        // Act
        var result = await _cryptoService.GetQuotaAsync(symbol);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ErrorType.UnknownError, result.ErrorType);
    }
}
