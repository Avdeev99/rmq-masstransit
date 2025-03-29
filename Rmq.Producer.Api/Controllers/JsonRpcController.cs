using Microsoft.AspNetCore.Mvc;
using Rmq.Core.Contracts;
using Rmq.Producer.Api.Services.Interfaces;

namespace Rmq.Producer.Api.Controllers;

[ApiController]
[Route("api/json-rpc")]
public class JsonRpcController : ControllerBase
{
    private readonly IMessageService _messageService;
    private readonly ILogger<JsonRpcController> _logger;

    public JsonRpcController(IMessageService messageService, ILogger<JsonRpcController> logger)
    {
        _messageService = messageService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> HandleJsonRpc([FromBody] JsonRpcRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Received JSON-RPC request: Method={Method}, Id={Id}", 
            request.Method,
            request.Id);

        if (string.IsNullOrEmpty(request.Method))
        {
            return BadRequest(new JsonRpcResponse
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
            var response = await _messageService.SendMessageAsync(request, cancellationToken);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing JSON-RPC request");

            return StatusCode(500, new JsonRpcResponse
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
} 