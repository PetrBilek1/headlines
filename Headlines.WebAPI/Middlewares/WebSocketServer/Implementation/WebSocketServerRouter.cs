using System.Collections.Concurrent;
using Headlines.WebAPI.Middlewares.WebSocketServer.Abstraction;

namespace Headlines.WebAPI.Middlewares.WebSocketServer.Implementation
{
    public sealed class WebSocketServerRouter : IWebSocketServerRouter
    {
        private readonly ConcurrentDictionary<string, HashSet<Guid>> _listeners = new();

        public void AddListener(string actionKey, Guid connectionId)
        {
            if (!_listeners.ContainsKey(actionKey))
            {
                _listeners.TryAdd(actionKey, new HashSet<Guid>());
            }

            _listeners[actionKey].Add(connectionId);
        }

        public void RemoveListener(string actionKey, Guid connectionId)
        {
            if (_listeners.TryGetValue(actionKey, out var connectionIds))
            {
                connectionIds.Remove(connectionId);
            }
        }

        public void RemoveListenersOfConnection(Guid connectionId)
        {
            foreach (var connectionIds in _listeners.Values)
            {
                connectionIds.Remove(connectionId);
            }
        }

        public ICollection<Guid> GetRoutes(string actionKey)
        {
            if (_listeners.TryGetValue(actionKey, out var connectionIds))
                return connectionIds;

            return new HashSet<Guid>();
        }
    }
}