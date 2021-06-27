using Microsoft.AspNetCore.Http;
using PPCC.Service;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace PPCC.Api
{
    /// <summary>
    /// ASPNet Core middleware for handling incoming <see cref="WebSocket"/> connections.
    /// </summary>
    public class BroadcastWebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IBroadcastService _broadcastService;

        public BroadcastWebSocketMiddleware(RequestDelegate next, IBroadcastService broadcastService)
        {
            _next = next;
            _broadcastService = broadcastService;
        }

        public async Task Invoke(HttpContext context)
        {
            // If the incoming request is not a socket request or is not on our specified uri, it is not for this middleware.
            // Let someone else figure out what to do with it.
            if (!context.WebSockets.IsWebSocketRequest || !string.Equals(context.Request.Path, "/ProductionPlanNotificationService", StringComparison.OrdinalIgnoreCase))
            {
                await _next.Invoke(context);
                return;
            }

            var socket = await context.WebSockets.AcceptWebSocketAsync().ConfigureAwait(false);
            
            // Register the socket with the broadcaster.
            _broadcastService.Subscribe(socket);

            // Loop listening in on the socket.
            while (socket.State == WebSocketState.Open)
            {
                try
                { 
                    // Here we would normally set up a listener for incoming messages but for this coding
                    // challenge it is assumed the client will not send anything.
                }
                catch (WebSocketException e)
                {
                    if (e.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
                    {
                        socket.Abort();
                    }
                }
            }

            // When the socket was closed or an exception occured, the socket must be unregistered
            // from the broadcaster.
            _broadcastService.Unsubscribe(socket);
        }
    }
}
