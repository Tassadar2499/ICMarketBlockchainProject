namespace ICMarkets.Blockchains.Infrastructure.Persistence;

internal static class SqliteFileSystem
{
    public static void EnsureDatabaseDirectoryExists(string connectionString)
    {
        var dataSource = GetDataSource(connectionString);
        if (string.IsNullOrWhiteSpace(dataSource) || dataSource.Equals(":memory:", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (dataSource.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var fullPath = Path.GetFullPath(dataSource);
        var directory = Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    private static string? GetDataSource(string connectionString)
    {
        var parts = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var dataSourcePart = parts.FirstOrDefault(part =>
            part.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase) ||
            part.StartsWith("DataSource=", StringComparison.OrdinalIgnoreCase));

        if (dataSourcePart is null)
        {
            return null;
        }

        var separatorIndex = dataSourcePart.IndexOf('=');
        return separatorIndex < 0 ? null : dataSourcePart[(separatorIndex + 1)..].Trim();
    }
}
