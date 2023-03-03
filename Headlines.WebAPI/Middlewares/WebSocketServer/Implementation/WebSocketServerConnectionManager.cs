using System.Collections.Concurrent;
using System.Net.WebSockets;
using Headlines.WebAPI.Middlewares.WebSocketServer.Abstraction;

namespace Headlines.WebAPI.Middlewares.WebSocketServer.Implementation
{
    public sealed class WebSocketServerConnectionManager : IWebSocketServerConnectionManager
    {
        private readonly ConcurrentDictionary<Guid, WebSocket> _sockets = new();

        public ConcurrentDictionary<Guid, WebSocket> GetAllSockets() => _sockets;

        public Guid AddSocket(WebSocket socket)
        {
            var connectionId = Guid.NewGuid();

            _sockets.TryAdd(connectionId, socket);

            return connectionId;
        }

        public WebSocket? RemoveSocket(Guid connectionId)
        {
            _sockets.TryRemove(connectionId, out var socket);

            return socket;
        }
    }
}