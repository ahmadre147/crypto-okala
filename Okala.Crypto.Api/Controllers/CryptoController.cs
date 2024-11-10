using Okala.Crypto.Domain.Services;

namespace Okala.Crypto.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class CryptoController(ICryptoService cryptoService, ILogger<CryptoController> logger) : BaseController
{
    [HttpGet("GetQuote")]
    public async Task<IActionResult> GetQuote(string symbol)
    {
        logger.LogInformation("Received request to fetch quota for symbol: {}", symbol);

        var result = await cryptoService.GetQuotaAsync(symbol);

        return ServerResponse(result);
    }
}
