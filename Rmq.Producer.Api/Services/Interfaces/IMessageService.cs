using Rmq.Core.Contracts;

namespace Rmq.Producer.Api.Services.Interfaces;

public interface IMessageService<TRequest, TResponse> where TRequest : JsonRpcRequestBase where TResponse : JsonRpcResponseBase
{
    Task<TResponse> SendMessageAsync(
        TRequest request,
        CancellationToken cancellationToken = default);
}

