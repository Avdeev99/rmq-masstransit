using System.Text.Json;

namespace Rmq.Core.Contracts;


public record JsonRpcRequestBase
{
    public string JsonRpc { get; init; } = "2.0";

    public string Id { get; init; } = Guid.NewGuid().ToString();

    public string Method { get; init; } = string.Empty;

    public virtual JsonElement? Params { get; set; }
}

public record JsonRpcRequest<TParams> : JsonRpcRequestBase where TParams : class
{    
    public new TParams? Params { get; init; }
}

public record JsonRpcResponseBase
{
    public string JsonRpc { get; init; } = "2.0";

    public string Id { get; init; } = Guid.NewGuid().ToString();
}

public record JsonRpcResponse<TResult> : JsonRpcResponseBase where TResult : class
{
    public TResult? Result { get; init; }

    public JsonRpcError? Error { get; init; }
}

public record JsonRpcError
{
    public int Code { get; init; }

    public string Message { get; init; } = string.Empty;

    public string? Data { get; init; }
}