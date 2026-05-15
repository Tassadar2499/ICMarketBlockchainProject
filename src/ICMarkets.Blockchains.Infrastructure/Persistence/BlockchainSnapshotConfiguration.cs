using ICMarkets.Blockchains.Domain.Snapshots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMarkets.Blockchains.Infrastructure.Persistence;

internal sealed class BlockchainSnapshotConfiguration : IEntityTypeConfiguration<BlockchainSnapshot>
{
    public void Configure(EntityTypeBuilder<BlockchainSnapshot> builder)
    {
        builder.ToTable("BlockchainSnapshots");

        builder.HasKey(snapshot => snapshot.Id);

        builder.Property(snapshot => snapshot.Chain)
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(snapshot => snapshot.Network)
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(snapshot => snapshot.SourceUrl)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(snapshot => snapshot.RawJson)
            .IsRequired();

        builder.Property(snapshot => snapshot.CreatedAt)
            .HasConversion(
                createdAt => createdAt.ToUnixTimeMilliseconds(),
                unixMilliseconds => DateTimeOffset.FromUnixTimeMilliseconds(unixMilliseconds))
            .IsRequired();

        builder.HasIndex(snapshot => new { snapshot.Chain, snapshot.Network, snapshot.CreatedAt })
            .HasDatabaseName("IX_BlockchainSnapshots_Chain_Network_CreatedAt");
    }
}
