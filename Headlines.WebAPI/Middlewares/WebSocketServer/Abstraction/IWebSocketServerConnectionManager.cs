using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace Headlines.WebAPI.Middlewares.WebSocketServer.Abstraction
{
    public interface IWebSocketServerConnectionManager
    {
        ConcurrentDictionary<Guid, WebSocket> GetAllSockets();
        Guid AddSocket(WebSocket socket);
        WebSocket? RemoveSocket(Guid connectionId);
    }
}