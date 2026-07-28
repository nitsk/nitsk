using System;
using System.Threading;
using Autodesk.Revit.UI;
using RevitMcpAddin.Commands;

namespace RevitMcpAddin
{
    /// <summary>
    /// Bridges the background HTTP thread to Revit's UI thread. Revit API calls are only legal on
    /// the UI thread, so incoming requests are handed off via ExternalEvent and the calling HTTP
    /// thread blocks on a wait handle until Execute(UIApplication) has run and set the response.
    /// Requests are serialized (one at a time) via the lock, matching a single-client MCP bridge.
    /// </summary>
    public class CommandDispatcher : IExternalEventHandler
    {
        private readonly object _requestLock = new object();
        private readonly ManualResetEventSlim _responseReady = new ManualResetEventSlim(false);
        private ExternalEvent _externalEvent;

        private RevitRequest _pendingRequest;
        private RevitResponse _pendingResponse;

        public void AttachExternalEvent(ExternalEvent externalEvent)
        {
            _externalEvent = externalEvent;
        }

        public RevitResponse Dispatch(RevitRequest request, TimeSpan timeout)
        {
            lock (_requestLock)
            {
                _pendingRequest = request;
                _pendingResponse = null;
                _responseReady.Reset();

                _externalEvent.Raise();

                if (!_responseReady.Wait(timeout))
                    return RevitResponse.Fail($"Timed out waiting {timeout.TotalSeconds:0}s for Revit to handle '{request.Command}'.");

                return _pendingResponse;
            }
        }

        void IExternalEventHandler.Execute(UIApplication app)
        {
            try
            {
                if (!CommandRegistry.TryGet(_pendingRequest.Command, out var command))
                {
                    _pendingResponse = RevitResponse.Fail($"Unknown command '{_pendingRequest.Command}'.");
                    return;
                }

                var result = command.Execute(app, _pendingRequest.Params);
                _pendingResponse = RevitResponse.Ok(result);
            }
            catch (Exception ex)
            {
                _pendingResponse = RevitResponse.Fail(ex.Message);
            }
            finally
            {
                _responseReady.Set();
            }
        }

        string IExternalEventHandler.GetName() => "Revit MCP Bridge";
    }
}
