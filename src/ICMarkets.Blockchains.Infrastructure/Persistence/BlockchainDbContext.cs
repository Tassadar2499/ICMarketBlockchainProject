using ICMarkets.Blockchains.Application.Abstractions.Data;
using ICMarkets.Blockchains.Domain.Snapshots;
using Microsoft.EntityFrameworkCore;

namespace ICMarkets.Blockchains.Infrastructure.Persistence;

public sealed class BlockchainDbContext(DbContextOptions<BlockchainDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<BlockchainSnapshot> BlockchainSnapshots => Set<BlockchainSnapshot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BlockchainDbContext).Assembly);
    }
}
