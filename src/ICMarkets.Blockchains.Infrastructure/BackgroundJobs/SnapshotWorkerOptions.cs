namespace ICMarkets.Blockchains.Infrastructure.BackgroundJobs;

public sealed class SnapshotWorkerOptions
{
    public const string SectionName = "SnapshotWorker";

    public bool Enabled { get; init; } = true;

    public TimeSpan Interval { get; init; } = TimeSpan.FromMinutes(5);
}
