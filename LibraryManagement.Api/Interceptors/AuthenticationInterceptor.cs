using Grpc.Core;
using Grpc.Core.Interceptors;

namespace LibraryManagement.Api.Interceptors;

public class AuthenticationInterceptor : Interceptor
{
    private const string headerName = "x-api-key";
    private readonly IConfiguration _configuration;

    public AuthenticationInterceptor(
        IConfiguration configuration
    )
    {
        _configuration = configuration;
    }

     public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var apiKey = context.RequestHeaders
            .FirstOrDefault(h => h.Key == headerName)?.Value;

        var expectedKey = _configuration["ApiKey"];

        if (string.IsNullOrEmpty(apiKey) || apiKey != expectedKey)
        {
            throw new RpcException(new Status(
                StatusCode.Unauthenticated,
                "Invalid or missing API key"));
        }

        return await continuation(request, context);
    }
}
