using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProductionPlanListener
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ClientWebSocket webSocket = null;

            try
            {
                webSocket = new ClientWebSocket();
                await webSocket.ConnectAsync(new Uri("ws://localhost:8888/ProductionPlanNotificationService"), CancellationToken.None);
                await Task.WhenAll(Receive(webSocket));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex);
            }
            finally
            {
                if (webSocket != null)
                    webSocket.Dispose();
                Console.WriteLine();
            }
        }

        private static async Task Receive(ClientWebSocket webSocket)
        {
            try
            {
                byte[] buffer = new byte[8192];
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    }
                    else
                    {
                        // For this coding challenge, we just dump the content.
                        Console.WriteLine(Encoding.UTF8.GetString(buffer));
                    }
                }
            }
            catch (WebSocketException)
            {
                Console.WriteLine("Something unexpected happened on the other side. Restart the application");
                webSocket.Dispose();
            }
        }

    }
}
