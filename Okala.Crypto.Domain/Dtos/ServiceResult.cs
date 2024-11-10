using Okala.Crypto.Domain.Enumeration;

namespace Okala.Crypto.Domain.Dtos;

public class ServiceResult<T>
{
    public T Data { get; set; }
    public ErrorType ErrorMessage { get; set; }
    public string ErrorMessageText { get; set; }
    public bool Success { get; private set; }
    
    public ServiceResult(ErrorType errorType)
    {
        this.ErrorMessage = errorType;
        this.Success = false;
    }
    
    public ServiceResult(string errorType)
    {
        this.ErrorMessageText = errorType;
        this.Success = false;
    }
    
    public ServiceResult(T data)
    {
        this.Data = data;
        this.Success=true;
    }
}
