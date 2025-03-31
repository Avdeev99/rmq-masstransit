using MassTransit;
using Rmq.Core.Contracts;
using Rmq.Core.Contracts.Requests;
using Rmq.Core.Contracts.Responses;

namespace Rmq.Consumer.Api.Consumers;

public class GetUserConsumer : IConsumer<JsonRpcRequest<GetUserRequest>>
{
    private readonly ILogger<GetUserConsumer> _logger;

    public GetUserConsumer(ILogger<GetUserConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<JsonRpcRequest<GetUserRequest>> context)
    {
        var request = context.Message;
        
        _logger.LogInformation(
            "Received JSON-RPC request: Method={Method}, Id={Id}", 
            request.Method,
            request.Id);

        var response = new JsonRpcResponse<GetUserResponse>
        {   
            Id = request.Id,
            Result = new GetUserResponse(Guid.NewGuid().ToString(), "John Doe", "john.doe@example.com")
        };
        
        _logger.LogInformation("Sending JSON-RPC response for request ID: {Id}", response.Id);
        
        await context.RespondAsync(response);
    }
} 