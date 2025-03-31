using Microsoft.AspNetCore.Mvc;
using Rmq.Core.Contracts;
using Rmq.Core.Contracts.Requests;
using Rmq.Core.Contracts.Responses;
using Rmq.Producer.Api.Services.Interfaces;
using System.Text.Json;

namespace Rmq.Producer.Api.Controllers;

[ApiController]
[Route("api/json-rpc")]
public class JsonRpcController : ControllerBase
{
    private readonly ILogger<JsonRpcController> _logger;
    private readonly IServiceProvider _serviceProvider;

    public JsonRpcController(
        ILogger<JsonRpcController> logger, 
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    [HttpPost]
    public async Task<IActionResult> HandleJsonRpc(
        [FromBody] JsonRpcRequestBase request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Received JSON-RPC request: Method={Method}, Id={Id}", 
            request.Method,
            request.Id);

        if (string.IsNullOrEmpty(request.Method))
        {
            return BadRequest(new JsonRpcResponse<GetUserResponse>
            {
                Id = request.Id,
                Error = new JsonRpcError
                {
                    Code = -32600,
                    Message = "Invalid Request: Method is required"
                }
            });
        }

        try
        {
            var response = await HandleRequestAsync(request, cancellationToken);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing JSON-RPC request");

            return StatusCode(500, new JsonRpcResponse<GetUserResponse>
            {
                Id = request.Id,
                Error = new JsonRpcError
                {
                    Code = -32603,
                    Message = "Internal error",
                    Data = ex.Message
                }
            });
        }
    }

    private async Task<JsonRpcResponseBase> HandleRequestAsync(JsonRpcRequestBase request, CancellationToken cancellationToken)
    {
        switch (request.Method)
        {
            case "user.get":
                return await HandleGetUserRequestAsync(request, cancellationToken);
            default:
                throw new Exception($"Method '{request.Method}' not found");
        }
    }

    private async Task<JsonRpcResponseBase> HandleGetUserRequestAsync(JsonRpcRequestBase request, CancellationToken cancellationToken)
    {
        var getUserRequest = new JsonRpcRequest<GetUserRequest>
        {
            Id = request.Id,
            JsonRpc = request.JsonRpc,
            Method = request.Method,
            Params = request.Params?.Deserialize<GetUserRequest>()
        };

        var messageService = _serviceProvider.GetRequiredService<IMessageService<JsonRpcRequest<GetUserRequest>, JsonRpcResponse<GetUserResponse>>>();
        var response = await messageService.SendMessageAsync(getUserRequest, cancellationToken);

        return response;
    }
}