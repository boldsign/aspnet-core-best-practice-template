namespace DemoApi.Service;

using DemoApi.Models.Request;

public interface IGrpcClientService
{
    Task<string> GetAuthTokenAsync(TokenRequest request);
}
