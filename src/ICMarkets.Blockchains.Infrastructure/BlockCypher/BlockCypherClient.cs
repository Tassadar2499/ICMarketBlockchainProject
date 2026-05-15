using System.Net.Http.Headers;
using System.Text.Json;
using ICMarkets.Blockchains.Application.Abstractions.External;
using ICMarkets.Blockchains.Application.Common;
using Microsoft.Extensions.Options;

namespace ICMarkets.Blockchains.Infrastructure.BlockCypher;

public sealed class BlockCypherClient(
    HttpClient httpClient,
    IOptions<BlockCypherOptions> options)
    : IBlockCypherClient
{
    public async Task<string> FetchSnapshotJsonAsync(
        BlockchainSource source,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, BuildUri(source.Url, options.Value.ApiToken));
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        var rawJson = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"BlockCypher returned {(int)response.StatusCode} {response.ReasonPhrase}: {rawJson}");
        }

        using var _ = JsonDocument.Parse(rawJson);
        return rawJson;
    }

    private static Uri BuildUri(string sourceUrl, string? apiToken)
    {
        if (string.IsNullOrWhiteSpace(apiToken))
        {
            return new Uri(sourceUrl, UriKind.Absolute);
        }

        var separator = sourceUrl.Contains('?', StringComparison.Ordinal) ? '&' : '?';
        return new Uri($"{sourceUrl}{separator}token={Uri.EscapeDataString(apiToken)}", UriKind.Absolute);
    }
}
