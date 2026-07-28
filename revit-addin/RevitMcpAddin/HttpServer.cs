using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace RevitMcpAddin
{
    /// <summary>
    /// A minimal loopback-only JSON/HTTP server. Accepts POST requests with a RevitRequest JSON
    /// body and returns a RevitResponse JSON body. Runs entirely on a background thread; actual
    /// Revit API work is delegated to CommandDispatcher, which hops back onto the UI thread.
    /// </summary>
    public class HttpServer
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
        };

        private readonly HttpListener _listener = new HttpListener();
        private readonly CommandDispatcher _dispatcher;
        private readonly TimeSpan _commandTimeout;
        private CancellationTokenSource _cts;

        public HttpServer(CommandDispatcher dispatcher, int port, TimeSpan? commandTimeout = null)
        {
            _dispatcher = dispatcher;
            _commandTimeout = commandTimeout ?? TimeSpan.FromSeconds(30);
            // Loopback only: this bridge is not meant to be reachable from outside the machine.
            _listener.Prefixes.Add($"http://127.0.0.1:{port}/");
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            _listener.Start();
            Task.Run(() => ListenLoop(_cts.Token));
        }

        public void Stop()
        {
            _cts?.Cancel();
            if (_listener.IsListening)
                _listener.Stop();
        }

        private async Task ListenLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested && _listener.IsListening)
            {
                HttpListenerContext context;
                try
                {
                    context = await _listener.GetContextAsync().ConfigureAwait(false);
                }
                catch (Exception) when (token.IsCancellationRequested || !_listener.IsListening)
                {
                    return;
                }

                _ = Task.Run(() => HandleRequest(context));
            }
        }

        private void HandleRequest(HttpListenerContext context)
        {
            RevitResponse response;
            try
            {
                if (context.Request.HttpMethod != "POST")
                {
                    response = RevitResponse.Fail("Only POST is supported.");
                }
                else
                {
                    string body;
                    using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                        body = reader.ReadToEnd();

                    var request = JsonSerializer.Deserialize<RevitRequest>(body, JsonOptions);
                    response = _dispatcher.Dispatch(request, _commandTimeout);
                }
            }
            catch (Exception ex)
            {
                response = RevitResponse.Fail($"Bridge error: {ex.Message}");
            }

            WriteResponse(context, response);
        }

        private static void WriteResponse(HttpListenerContext context, RevitResponse response)
        {
            try
            {
                var json = JsonSerializer.Serialize(response, JsonOptions);
                var bytes = Encoding.UTF8.GetBytes(json);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = response.Success ? 200 : 400;
                context.Response.ContentLength64 = bytes.Length;
                context.Response.OutputStream.Write(bytes, 0, bytes.Length);
            }
            finally
            {
                context.Response.OutputStream.Close();
            }
        }
    }
}
