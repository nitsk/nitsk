using Autodesk.Revit.UI;

namespace RevitMcpAddin
{
    /// <summary>
    /// Loaded once when Revit starts (registered via RevitMcpAddin.addin). Starts a loopback HTTP
    /// server for the lifetime of the Revit session so the Python MCP server (mcp-server/) can
    /// drive the active document.
    /// </summary>
    public class App : IExternalApplication
    {
        // Keep in sync with mcp-server/revit_mcp/client.py's default port.
        private const int Port = 8787;

        private CommandDispatcher _dispatcher;
        private ExternalEvent _externalEvent;
        private HttpServer _httpServer;

        public Result OnStartup(UIControlledApplication application)
        {
            _dispatcher = new CommandDispatcher();
            _externalEvent = ExternalEvent.Create(_dispatcher);
            _dispatcher.AttachExternalEvent(_externalEvent);

            _httpServer = new HttpServer(_dispatcher, Port);
            _httpServer.Start();

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            _httpServer?.Stop();
            _externalEvent?.Dispose();
            return Result.Succeeded;
        }
    }
}
