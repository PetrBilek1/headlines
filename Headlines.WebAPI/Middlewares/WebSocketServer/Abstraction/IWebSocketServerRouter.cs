namespace Headlines.WebAPI.Middlewares.WebSocketServer.Abstraction
{
    public interface IWebSocketServerRouter
    {
        void AddListener(WebSocketServerRouterAction action, Guid connectionId);
        void RemoveListener(WebSocketServerRouterAction action, Guid connectionId);
        void RemoveListenersOfConnection(Guid connectionId);
        ICollection<Guid> GetRoutes(WebSocketServerRouterAction action);
    }
}