using Okala.Crypto.Domain.Dtos;
using Okala.Crypto.Domain.Enumeration;

namespace Okala.Crypto.Services;

internal class ServiceBase
{
    protected ServiceResult<TOut> ErrorResult<TOut>(ErrorType error)
    {
        return new ServiceResult<TOut>(error);
    }

    protected ServiceResult<TOut> ErrorResult<TOut>(string error)
    {
        return new ServiceResult<TOut>(error);
    }

    protected ServiceResult<TOut> SuccessResult<TOut>(TOut data)
    {
        return new ServiceResult<TOut>(data);
    }
}
