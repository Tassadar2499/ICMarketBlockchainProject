namespace ICMarkets.Blockchains.Application.Abstractions.Messaging;

public interface ICommandHandler<in TCommand, TResponse>
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
