using Grpc.Core;
using Grpc.Core.Interceptors;
using Serilog;

public class LoggingInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        Log.Information("gRPC Request: {Method}, Payload: {@Request}", context.Method, request);

        try
        {
            var response = await continuation(request, context);
            Log.Information("gRPC Response: {@Response}", response);
            return response;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "gRPC Error: {Method}", context.Method);
            throw;
        }
    }
}
