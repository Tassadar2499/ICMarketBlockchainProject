using System.Net;
using System.Text.Json;
using ICMarkets.Blockchains.IntegrationTests.Support;

namespace ICMarkets.Blockchains.IntegrationTests;

public sealed class BlockchainEndpointsTests
{
    [Fact]
    public async Task Health_Returns_Ok()
    {
        using var server = BlockCypherMockServer.CreateWithDefaultResponses();
        using var factory = new ApiTestFactory(server);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PostSingleSnapshot_Stores_And_Returns_History()
    {
        using var server = BlockCypherMockServer.CreateWithDefaultResponses();
        using var factory = new ApiTestFactory(server);
        using var client = factory.CreateClient();

        var postResponse = await client.PostAsync("/api/blockchains/eth/main/snapshots", content: null);

        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        using var postJson = JsonDocument.Parse(await postResponse.Content.ReadAsStringAsync());
        Assert.Equal("eth", postJson.RootElement.GetProperty("chain").GetString());
        Assert.Equal("main", postJson.RootElement.GetProperty("network").GetString());
        Assert.Equal("ETH.main", postJson.RootElement.GetProperty("data").GetProperty("name").GetString());

        var historyResponse = await client.GetAsync("/api/blockchains/eth/main/snapshots?page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, historyResponse.StatusCode);

        using var historyJson = JsonDocument.Parse(await historyResponse.Content.ReadAsStringAsync());
        Assert.Equal(1, historyJson.RootElement.GetProperty("totalCount").GetInt32());
        Assert.Single(historyJson.RootElement.GetProperty("items").EnumerateArray());
    }

    [Fact]
    public async Task PostAllSnapshots_Stores_All_Configured_Sources()
    {
        using var server = BlockCypherMockServer.CreateWithDefaultResponses();
        using var factory = new ApiTestFactory(server);
        using var client = factory.CreateClient();

        var response = await client.PostAsync("/api/blockchains/snapshots", content: null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        Assert.Equal(5, json.RootElement.GetProperty("snapshots").GetArrayLength());
        Assert.Equal(0, json.RootElement.GetProperty("failures").GetArrayLength());
    }

    [Fact]
    public async Task History_Returns_Newest_Snapshot_First_With_Pagination()
    {
        using var server = BlockCypherMockServer.CreateWithDefaultResponses();
        using var factory = new ApiTestFactory(server);
        using var client = factory.CreateClient();

        server.SetJson("/v1/btc/main", "{\"name\":\"BTC.main\",\"height\":10}");
        var firstResponse = await client.PostAsync("/api/blockchains/btc/main/snapshots", content: null);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        await Task.Delay(25);

        server.SetJson("/v1/btc/main", "{\"name\":\"BTC.main\",\"height\":20}");
        var secondResponse = await client.PostAsync("/api/blockchains/btc/main/snapshots", content: null);
        Assert.Equal(HttpStatusCode.Created, secondResponse.StatusCode);

        var historyResponse = await client.GetAsync("/api/blockchains/btc/main/snapshots?page=1&pageSize=1");

        Assert.Equal(HttpStatusCode.OK, historyResponse.StatusCode);
        using var historyJson = JsonDocument.Parse(await historyResponse.Content.ReadAsStringAsync());

        Assert.Equal(2, historyJson.RootElement.GetProperty("totalCount").GetInt32());
        Assert.Equal(2, historyJson.RootElement.GetProperty("totalPages").GetInt32());

        var firstItem = historyJson.RootElement.GetProperty("items").EnumerateArray().Single();
        Assert.Equal(20, firstItem.GetProperty("data").GetProperty("height").GetInt32());
    }
}
