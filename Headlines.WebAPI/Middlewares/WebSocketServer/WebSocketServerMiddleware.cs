using Headlines.WebAPI.Middlewares.WebSocketServer.Abstraction;
using System.Net.WebSockets;

namespace Headlines.WebAPI.Middlewares.WebSocketServer
{
    public sealed class WebSocketServerMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly IWebSocketServerService _server;

        public WebSocketServerMiddleware(RequestDelegate next, IWebSocketServerService server)
        {
            _next = next;
            _server = server;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next(context);
                return;
            }

            WebSocket socket = await context.WebSockets.AcceptWebSocketAsync();
            var connectionId = _server.AddSocket(socket);

            await ReceiveMessage(socket, async (result, buffer) =>
            {
                switch (result.MessageType)
                {
                    case WebSocketMessageType.Text:
                        _server.ConsumeTextMessage(connectionId, result, buffer);
                        break;
                    case WebSocketMessageType.Close:
                        await _server.ConsumeCloseMessageAsync(connectionId, result);
                        break;
                }
            });
        }

        private static async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer), cancellationToken: CancellationToken.None);
                handleMessage(result, buffer);
            }
        }        
    }
}