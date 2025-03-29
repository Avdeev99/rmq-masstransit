using System.Text.Json;
using MassTransit;
using Rmq.Consumer.Api.Contracts;

namespace Rmq.Consumer.Api.Consumers;

public class JsonRpcConsumer : IConsumer<JsonRpcRequest>
{
    private readonly ILogger<JsonRpcConsumer> _logger;

    public JsonRpcConsumer(ILogger<JsonRpcConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<JsonRpcRequest> context)
    {
        var request = context.Message;
        
        _logger.LogInformation(
            "Received JSON-RPC request: Method={Method}, Id={Id}", 
            request.Method,
            request.Id);

        JsonRpcResponse response;
        
        try
        {
            response = request.Method.ToLowerInvariant() switch
            {
                "processmessage" => ProcessMessage(request),
                _ => new JsonRpcResponse
                {
                    Id = request.Id,
                    Error = new JsonRpcError
                    {
                        Code = -32601,
                        Message = $"Method '{request.Method}' not found"
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing JSON-RPC request");
            response = new JsonRpcResponse
            {
                Id = request.Id,
                Error = new JsonRpcError
                {
                    Code = -32603,
                    Message = "Internal error",
                    Data = ex.Message
                }
            };
        }
        
        _logger.LogInformation("Sending JSON-RPC response for request ID: {Id}", response.Id);
        
        await context.RespondAsync(response);
    }
    
    private JsonRpcResponse ProcessMessage(JsonRpcRequest request)
    {
        try
        {
            string serializedParams = JsonSerializer.Serialize(request.Params, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            
            _logger.LogInformation(
                "Method '{Method}' called with params: {Params}", 
                request.Method, serializedParams);
            
            return new JsonRpcResponse
            {
                Id = request.Id,
                Result = new
                {
                    Method = request.Method,
                    Params = request.Params,
                    Success = true,
                    Timestamp = DateTime.UtcNow
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing JSON-RPC request params for method '{Method}'", 
                request.Method);
            return new JsonRpcResponse
            {
                Id = request.Id,
                Error = new JsonRpcError
                {
                    Code = -32603,
                    Message = "Error processing content",
                    Data = ex.Message
                }
            };
        }
    }
} 