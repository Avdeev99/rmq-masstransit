namespace Rmq.Core.Contracts;

public record JsonRpcRequest
{
    public string JsonRpc { get; init; } = "2.0";

    public string Id { get; init; } = Guid.NewGuid().ToString();

    public string Method { get; init; } = string.Empty;

    public object? Params { get; init; }
}

public record JsonRpcResponse
{
    public string JsonRpc { get; init; } = "2.0";

    public string Id { get; init; } = string.Empty;

    public object? Result { get; init; }

    public JsonRpcError? Error { get; init; }
}

public record JsonRpcError
{
    public int Code { get; init; }

    public string Message { get; init; } = string.Empty;

    public string? Data { get; init; }
}