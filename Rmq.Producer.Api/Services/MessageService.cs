using MassTransit;
using Rmq.Core.Contracts;
using Rmq.Producer.Api.Services.Interfaces;

namespace Rmq.Producer.Api.Services;

public class MessageService<TRequest, TResponse> : IMessageService<TRequest, TResponse>
    where TRequest : JsonRpcRequestBase where TResponse : JsonRpcResponseBase
{
    private readonly IRequestClient<TRequest> _requestClient;
    private readonly ILogger<MessageService<TRequest, TResponse>> _logger;

    public MessageService(IRequestClient<TRequest> requestClient, ILogger<MessageService<TRequest, TResponse>> logger)
    {
        _requestClient = requestClient;
        _logger = logger;
    }

    public async Task<TResponse> SendMessageAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending JSON-RPC request: Method={Method}, Id={Id}", request.Method, request.Id);

        var response = await _requestClient.GetResponse<TResponse>(request, cancellationToken);
        
        _logger.LogInformation("Received JSON-RPC response for request ID: {Id}", response.Message.Id);
            
        return response.Message;
    }
} 