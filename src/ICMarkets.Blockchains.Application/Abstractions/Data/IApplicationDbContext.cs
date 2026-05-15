using ICMarkets.Blockchains.Domain.Snapshots;
using Microsoft.EntityFrameworkCore;

namespace ICMarkets.Blockchains.Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<BlockchainSnapshot> BlockchainSnapshots { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
