using Okala.Crypto.Domain.Dtos;
using Okala.Crypto.Domain.Enumeration;

namespace Okala.Crypto.Services;

/// <summary>
/// Base service class providing helper methods for creating standardized service results.
/// </summary>
internal class ServiceBase
{
    /// <summary>
    /// Creates an error result with a predefined error type.
    /// </summary>
    /// <typeparam name="TOut">The type of the data expected in the service result.</typeparam>
    /// <param name="error">The type of error to include in the result.</param>
    /// <returns>A <see cref="ServiceResult{TOut}"/> representing an error outcome.</returns>
    protected ServiceResult<TOut> ErrorResult<TOut>(ErrorType error)
    {
        return new ServiceResult<TOut>(error);
    }

    /// <summary>
    /// Creates an error result with a custom error message.
    /// </summary>
    /// <typeparam name="TOut">The type of the data expected in the service result.</typeparam>
    /// <param name="error">A custom error message to include in the result.</param>
    /// <returns>A <see cref="ServiceResult{TOut}"/> representing an error outcome.</returns>
    protected ServiceResult<TOut> ErrorResult<TOut>(string error)
    {
        return new ServiceResult<TOut>(error);
    }

    /// <summary>
    /// Creates a success result with the provided data.
    /// </summary>
    /// <typeparam name="TOut">The type of the data to include in the service result.</typeparam>
    /// <param name="data">The data to include in the successful result.</param>
    /// <returns>A <see cref="ServiceResult{TOut}"/> representing a successful outcome with data.</returns>
    protected ServiceResult<TOut> SuccessResult<TOut>(TOut data)
    {
        return new ServiceResult<TOut>(data);
    }
}