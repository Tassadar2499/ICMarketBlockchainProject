using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ICMarkets.Blockchains.IntegrationTests.Support;

public sealed class BlockCypherMockServer : IDisposable
{
    private readonly ConcurrentDictionary<string, string> _responses = new(StringComparer.OrdinalIgnoreCase);
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly HttpListener _listener = new();
    private readonly Task _listenerTask;

    public BlockCypherMockServer()
    {
        var port = GetAvailablePort();
        BaseAddress = $"http://127.0.0.1:{port}";

        _listener.Prefixes.Add($"{BaseAddress}/");
        _listener.Start();
        _listenerTask = Task.Run(ListenAsync);
    }

    public string BaseAddress { get; }

    public static BlockCypherMockServer CreateWithDefaultResponses()
    {
        var server = new BlockCypherMockServer();

        server.SetJson("/v1/eth/main", "{\"name\":\"ETH.main\",\"height\":1}");
        server.SetJson("/v1/dash/main", "{\"name\":\"DASH.main\",\"height\":2}");
        server.SetJson("/v1/btc/main", "{\"name\":\"BTC.main\",\"height\":3}");
        server.SetJson("/v1/btc/test3", "{\"name\":\"BTC.test3\",\"height\":4}");
        server.SetJson("/v1/ltc/main", "{\"name\":\"LTC.main\",\"height\":5}");

        return server;
    }

    public void SetJson(string path, string json)
    {
        _responses[path] = json;
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();

        if (_listener.IsListening)
        {
            _listener.Stop();
        }

        _listener.Close();
        _cancellationTokenSource.Dispose();

        try
        {
            _listenerTask.Wait(TimeSpan.FromSeconds(2));
        }
        catch (AggregateException)
        {
        }
    }

    private async Task ListenAsync()
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            HttpListenerContext context;

            try
            {
                context = await _listener.GetContextAsync();
            }
            catch (ObjectDisposedException)
            {
                break;
            }
            catch (HttpListenerException)
            {
                break;
            }

            _ = Task.Run(() => RespondAsync(context));
        }
    }

    private async Task RespondAsync(HttpListenerContext context)
    {
        var path = context.Request.Url?.AbsolutePath ?? "/";

        if (!_responses.TryGetValue(path, out var json))
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await WriteResponseAsync(context.Response, "{\"error\":\"not found\"}");
            return;
        }

        context.Response.StatusCode = (int)HttpStatusCode.OK;
        await WriteResponseAsync(context.Response, json);
    }

    private static async Task WriteResponseAsync(HttpListenerResponse response, string body)
    {
        response.ContentType = "application/json";
        var bytes = Encoding.UTF8.GetBytes(body);
        response.ContentLength64 = bytes.Length;
        await response.OutputStream.WriteAsync(bytes);
        response.OutputStream.Close();
    }

    private static int GetAvailablePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}
