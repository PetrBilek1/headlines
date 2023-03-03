using System.Net.WebSockets;

namespace Headlines.WebAPI.Middlewares.WebSocketServer.Abstraction
{
    public interface IWebSocketServerService
    {
        Guid AddSocket(WebSocket socket);
        void ConsumeTextMessage(Guid connectionId, WebSocketReceiveResult result, byte[] buffer);
        Task ConsumeCloseMessageAsync(Guid connectionId, WebSocketReceiveResult result);
        Task SendMessageByActionKeyAsync(string actionKey, string message);
    }
}