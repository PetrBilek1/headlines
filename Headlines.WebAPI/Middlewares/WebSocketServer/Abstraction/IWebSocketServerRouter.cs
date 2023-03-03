namespace Headlines.WebAPI.Middlewares.WebSocketServer.Abstraction
{
    public interface IWebSocketServerRouter
    {
        void AddListener(string actionKey, Guid connectionId);
        void RemoveListener(string actionKey, Guid connectionId);
        void RemoveListenersOfConnection(Guid connectionId);
        ICollection<Guid> GetRoutes(string actionKey);
    }
}