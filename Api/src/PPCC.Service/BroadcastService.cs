using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PPCC.Service
{
    /// <summary>
    /// Broadcasting service for <see cref="WebSocket"/> connections.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class BroadcastService : IBroadcastService
    {
        /// <summary>
        /// Internal collection with all the sockets.
        /// </summary>
        private readonly ConcurrentDictionary<string, WebSocket> _socketManager = new ConcurrentDictionary<string, WebSocket>();

        /// <summary>
        /// Add the <see cref="WebSocket"/> to the list of subscribers.
        /// </summary>
        public void Subscribe(WebSocket webSocket)
        {
            _socketManager.TryAdd(Guid.NewGuid().ToString(), webSocket);
        }

        /// <summary>
        /// Remove the <see cref="WebSocket"/> from the list of subscribers.
        /// </summary>
        public void Unsubscribe(WebSocket webSocket)
        {
            var key = _socketManager.FirstOrDefault(p => p.Value == webSocket).Key;

            if (key != null)
            {
                _socketManager.TryRemove(key, out WebSocket socket);
            }
        }

        /// <summary>
        /// Broadcasts a message to all subscribers.
        /// </summary>
        public async Task BroadcastMessageAsync(string message)
        {
            foreach (var webSocket in _socketManager.Values)
            {
                try
                {
                    if (webSocket.State == WebSocketState.Open)
                    {
                        var encodedMessage = Encoding.UTF8.GetBytes(message);
                        await webSocket.SendAsync(
                            new ArraySegment<byte>(encodedMessage, 0, encodedMessage.Length),
                            WebSocketMessageType.Text, true, CancellationToken.None).ConfigureAwait(false);
                    }
                }
                catch (WebSocketException e)
                {
                    if (e.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
                    {
                        Unsubscribe(webSocket);
                    }
                }
            }
        }
    }
}
