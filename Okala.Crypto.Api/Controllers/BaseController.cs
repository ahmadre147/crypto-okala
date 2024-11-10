using Microsoft.AspNetCore.Mvc;
using Okala.Crypto.Domain.Enumeration;

namespace Okala.Crypto.Api.Controllers;

public class BaseController : ControllerBase
{
    protected ActionResult ServerResponse<T>(Domain.Dtos.ServiceResult<T> input)
    {
        if (input.Success)
        {
            return Ok(input.Data);
        }

        return !string.IsNullOrEmpty(input.ErrorMessageText)
            ? ServerError(input.ErrorMessage, input.ErrorMessageText)
            : ServerError(input.ErrorMessage);
    }

    private ActionResult ServerError(ErrorType message, string messageText)
    {
        return BadRequest(messageText);
    }

    private ActionResult ServerError(ErrorType message)
    {
        return BadRequest(message.ToString());
    }
}