using System.Net;
using System.Text;
using System.Text.Json;
using ICMarkets.Blockchains.Application.Common;
using ICMarkets.Blockchains.Infrastructure.BlockCypher;
using Microsoft.Extensions.Options;

namespace ICMarkets.Blockchains.UnitTests;

public sealed class BlockCypherClientTests
{
    [Fact]
    public async Task FetchSnapshotJsonAsync_Appends_Optional_Token()
    {
        var handler = new RecordingHttpMessageHandler(HttpStatusCode.OK, "{\"height\":123}");
        var client = new BlockCypherClient(
            new HttpClient(handler),
            Options.Create(new BlockCypherOptions { ApiToken = "secret value" }));

        var rawJson = await client.FetchSnapshotJsonAsync(
            new BlockchainSource("btc", "main", "https://api.blockcypher.com/v1/btc/main"));

        Assert.Equal("{\"height\":123}", rawJson);
        Assert.NotNull(handler.RequestUri);
        Assert.Contains("token=secret%20value", handler.RequestUri!.Query);
    }

    [Fact]
    public async Task FetchSnapshotJsonAsync_Throws_For_Invalid_Json()
    {
        var handler = new RecordingHttpMessageHandler(HttpStatusCode.OK, "not-json");
        var client = new BlockCypherClient(new HttpClient(handler), Options.Create(new BlockCypherOptions()));

        await Assert.ThrowsAnyAsync<JsonException>(() => client.FetchSnapshotJsonAsync(
            new BlockchainSource("eth", "main", "https://api.blockcypher.com/v1/eth/main")));
    }

    private sealed class RecordingHttpMessageHandler(HttpStatusCode statusCode, string responseBody) : HttpMessageHandler
    {
        public Uri? RequestUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestUri = request.RequestUri;

            return Task.FromResult(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
            });
        }
    }
}
