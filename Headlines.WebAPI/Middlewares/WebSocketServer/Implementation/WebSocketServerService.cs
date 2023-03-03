using Headlines.WebAPI.Contracts.WebSockets;
using Headlines.WebAPI.Middlewares.WebSocketServer.Abstraction;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace Headlines.WebAPI.Middlewares.WebSocketServer.Implementation
{
    public class WebSocketServerService : IWebSocketServerService
    {
        private readonly IWebSocketServerRouter _router;
        private readonly IWebSocketServerConnectionManager _connectionManager;

        public WebSocketServerService(IWebSocketServerRouter router, IWebSocketServerConnectionManager connectionManager)
        {
            _router = router;
            _connectionManager = connectionManager;
        }

        public Guid AddSocket(WebSocket socket) => _connectionManager.AddSocket(socket);

        public void ConsumeTextMessage(Guid connectionId, WebSocketReceiveResult result, byte[] buffer)
        {
            var textContent = Encoding.UTF8.GetString(buffer, 0, result.Count);
            if (string.IsNullOrEmpty(textContent))
                return;

            var message = JsonConvert.DeserializeObject<ListenToActionMessage>(textContent);

            if (message?.ActionName == WebSocketServerRouterAction.ArticleDetailScraped.ActionName)
            {
                _router.AddListener(
                    WebSocketServerRouterAction
                    .ArticleDetailScraped.Create(Convert.ToInt64(message?.Parameter))
                    , connectionId);
            }
        }

        public async Task ConsumeCloseMessageAsync(Guid connectionId, WebSocketReceiveResult result)
        {
            _router.RemoveListenersOfConnection(connectionId);
            var socket = _connectionManager.RemoveSocket(connectionId);
            if (socket == null)
                return;

            await socket.CloseAsync(result.CloseStatus!.Value, result.CloseStatusDescription, default);
        }

        public async Task SendMessageByActionAsync(WebSocketServerRouterAction action, string message)
        {
            foreach (var route in _router.GetRoutes(action))
            {
                var socket = _connectionManager.GetAllSockets()[route];
                await socket.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, default);
            }
        }
    }
}