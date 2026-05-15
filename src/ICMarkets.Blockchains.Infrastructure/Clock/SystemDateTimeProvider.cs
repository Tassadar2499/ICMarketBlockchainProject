using ICMarkets.Blockchains.Application.Abstractions.Clock;

namespace ICMarkets.Blockchains.Infrastructure.Clock;

public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
