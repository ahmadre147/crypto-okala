using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Okala.Crypto.Domain.Dtos;
using Okala.Crypto.Domain.Dtos.Quota;
using Okala.Crypto.Domain.Enumeration;
using Okala.Crypto.Domain.Services;
using Okala.Crypto.Services;
using Okala.Crypto.Utils.Cache;

namespace Okala.Crypto.Test.CryptoServiceTests;

public class CryptoServiceTest
{
    private readonly Mock<ICacheProvider> _cacheProviderMock;
    private readonly Mock<IExchangeManager> _exchangeManagerMock;
    private readonly ICryptoService _cryptoService;

    public CryptoServiceTest()
    {
        _cacheProviderMock = new Mock<ICacheProvider>();
        _exchangeManagerMock = new Mock<IExchangeManager>();
        var logger = new NullLogger<CryptoService>();
        _cryptoService = new CryptoService(_cacheProviderMock.Object, _exchangeManagerMock.Object, logger);
    }

    [Fact]
    public async Task GetQuotaAsync_CacheHit_ReturnsCachedData()
    {
        var symbol = "BTC";
        var quotaData = new QuotaResponseDto
        {
            Prices =
            [
                new PricePairDto
                {
                    Currency = "USD",
                    Value = 80000
                }
            ]
        };
        _cacheProviderMock.Setup(cp => cp.GetAsync<QuotaResponseDto>("quota:BTC", default))
            .ReturnsAsync(quotaData);

        var result = await _cryptoService.GetQuotaAsync(symbol);

        Assert.NotNull(result);
        Assert.Equal(quotaData, result.Data);
        _cacheProviderMock.Verify(cp => cp.GetAsync<QuotaResponseDto>("quota:BTC", default), Times.Once);
        _exchangeManagerMock.Verify(em => em.GetQuotaAsync(It.IsAny<string>(), default), Times.Never);
    }

    [Fact]
    public async Task GetQuotaAsync_CacheMiss_FetchesFromExchangeAndCachesData()
    {
        var symbol = "BTC";
        var quotaData = new QuotaResponseDto
        {
            Prices =
            [
                new PricePairDto
                {
                    Currency = "USD",
                    Value = 80000
                }
            ]
        };
        _cacheProviderMock.Setup(cp => cp.GetAsync<QuotaResponseDto>("quota:BTC", default))
                          .ReturnsAsync((QuotaResponseDto?)null);
        _exchangeManagerMock.Setup(em => em.GetQuotaAsync(symbol, default))
                            .ReturnsAsync(new ServiceResult<QuotaResponseDto>(quotaData));

        var result = await _cryptoService.GetQuotaAsync(symbol);

        Assert.NotNull(result);
        Assert.Equal(quotaData, result.Data);
        _cacheProviderMock.Verify(cp => cp.GetAsync<QuotaResponseDto>("quota:BTC", default), Times.Once);
    }

    [Fact]
    public async Task GetQuotaAsync_Exception_ReturnsErrorResult()
    {
        var symbol = "BTC";
        _cacheProviderMock.Setup(cp => cp.GetAsync<QuotaResponseDto>("quota:BTC", default))
                          .Throws(new Exception("Cache error"));

        var result = await _cryptoService.GetQuotaAsync(symbol);

        Assert.NotNull(result);
        Assert.Equal(ErrorType.UnknownError, result.ErrorMessage);
    }
}
