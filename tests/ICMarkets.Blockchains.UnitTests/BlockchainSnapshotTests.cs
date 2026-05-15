using ICMarkets.Blockchains.Domain.Snapshots;

namespace ICMarkets.Blockchains.UnitTests;

public sealed class BlockchainSnapshotTests
{
    [Fact]
    public void Create_Normalizes_Chain_And_Network()
    {
        var createdAt = DateTimeOffset.Parse("2026-05-15T12:00:00Z");

        var snapshot = BlockchainSnapshot.Create(
            " BTC ",
            " MAIN ",
            "https://api.blockcypher.com/v1/btc/main",
            "{\"name\":\"BTC.main\"}",
            createdAt);

        Assert.Equal("btc", snapshot.Chain);
        Assert.Equal("main", snapshot.Network);
        Assert.Equal(createdAt, snapshot.CreatedAt);
    }

    [Fact]
    public void Create_Throws_When_RawJson_Is_Empty()
    {
        Assert.Throws<ArgumentException>(() => BlockchainSnapshot.Create(
            "btc",
            "main",
            "https://api.blockcypher.com/v1/btc/main",
            string.Empty,
            DateTimeOffset.UtcNow));
    }
}
