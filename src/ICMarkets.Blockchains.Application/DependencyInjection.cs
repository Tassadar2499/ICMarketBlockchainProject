using ICMarkets.Blockchains.Application.Abstractions.Messaging;
using ICMarkets.Blockchains.Application.Common;
using ICMarkets.Blockchains.Application.Snapshots.Commands.CreateAllSnapshots;
using ICMarkets.Blockchains.Application.Snapshots.Commands.CreateSnapshot;
using ICMarkets.Blockchains.Application.Snapshots.Queries.GetSnapshotHistory;
using Microsoft.Extensions.DependencyInjection;

namespace ICMarkets.Blockchains.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateBlockchainSnapshotCommand, Result<BlockchainSnapshotDto>>, CreateBlockchainSnapshotHandler>();
        services.AddScoped<ICommandHandler<CreateAllBlockchainSnapshotsCommand, CreateAllBlockchainSnapshotsResult>, CreateAllBlockchainSnapshotsHandler>();
        services.AddScoped<IQueryHandler<GetBlockchainSnapshotHistoryQuery, Result<PagedResult<BlockchainSnapshotDto>>>, GetBlockchainSnapshotHistoryHandler>();

        return services;
    }
}
