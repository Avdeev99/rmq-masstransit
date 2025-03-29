using MassTransit;
using Rmq.Core.Contracts;
using Rmq.Producer.Api.Services.Interfaces;

namespace Rmq.Producer.Api.Services;

public class MessageService : IMessageService
{
    private readonly IRequestClient<JsonRpcRequest> _requestClient;
    private readonly ILogger<MessageService> _logger;

    public MessageService(IRequestClient<JsonRpcRequest> requestClient, ILogger<MessageService> logger)
    {
        _requestClient = requestClient;
        _logger = logger;
    }

    public async Task<JsonRpcResponse> SendMessageAsync(JsonRpcRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending JSON-RPC request: Method={Method}, Id={Id}", request.Method, request.Id);

        var response = await _requestClient.GetResponse<JsonRpcResponse>(request, cancellationToken);
        
        _logger.LogInformation("Received JSON-RPC response for request ID: {Id}", response.Message.Id);
            
        return response.Message;
    }
} 