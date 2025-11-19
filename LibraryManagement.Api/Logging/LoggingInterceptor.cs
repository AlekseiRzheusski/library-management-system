using Grpc.Core;
using Grpc.Core.Interceptors;
using Serilog;

public class LoggingInterceptor : Interceptor
{
    private readonly ILogger<LoggingInterceptor> _logger;

    public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        _logger.LogInformation("gRPC Request: {Method}, Payload: {@Request}", context.Method, request);

        try
        {
            var response = await continuation(request, context);
            _logger.LogInformation("gRPC Response: {@Response}", response);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "gRPC Error: {Method}", context.Method);
            throw;
        }
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        _logger.LogInformation("gRPC ServerStreaming request: {Method}, Payload: {@Request}. Started...",
            context.Method, request);

        try
        {
            await continuation(request, responseStream, context);
            _logger.LogInformation("gRPC ServerStreaming method finished: {Method}", context.Method);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "gRPC ServerStreaming error: {Method}", context.Method);
            throw;
        }
    }
}
