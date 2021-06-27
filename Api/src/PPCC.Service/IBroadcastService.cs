using System.Net.WebSockets;
using System.Threading.Tasks;

namespace PPCC.Service
{
    /// <summary>
    /// Interface for web socket broadcasting.
    /// </summary>
    public interface IBroadcastService
    {
        /// <summary>
        /// Broadcasts a message to all subscribers.
        /// </summary>
        Task BroadcastMessageAsync(string message);

        /// <summary>
        /// Add the <see cref="WebSocket"/> to the list of subscribers.
        /// </summary>
        void Subscribe(WebSocket webSocket);

        /// <summary>
        /// Remove the <see cref="WebSocket"/> from the list of subscribers.
        /// </summary>
        void Unsubscribe(WebSocket webSocket);
    }
}