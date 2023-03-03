using System.Collections.Concurrent;
using Headlines.WebAPI.Middlewares.WebSocketServer.Abstraction;

namespace Headlines.WebAPI.Middlewares.WebSocketServer.Implementation
{
    public sealed class WebSocketServerRouter : IWebSocketServerRouter
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, HashSet<Guid>>> _listeners = new();

        public void AddListener(WebSocketServerRouterAction action, Guid connectionId)
        {
            if (!_listeners.ContainsKey(action.ActionName))
            {
                _listeners.TryAdd(action.ActionName, new ConcurrentDictionary<string, HashSet<Guid>>());
            }

            if (!_listeners[action.ActionName].ContainsKey(action.Parameter)) 
            {
                _listeners[action.ActionName].TryAdd(action.Parameter, new HashSet<Guid>());
            }

            _listeners[action.ActionName][action.Parameter].Add(connectionId);
        }

        public void RemoveListener(WebSocketServerRouterAction action, Guid connectionId)
        {
            if (_listeners.TryGetValue(action.ActionName, out var parameterDicts)
                && parameterDicts.TryGetValue(action.Parameter, out var connectionIds))
            {
                connectionIds.Remove(connectionId);
            }
        }

        public void RemoveListenersOfConnection(Guid connectionId)
        {
            foreach (var connectionIds in _listeners.Values.SelectMany(x => x.Values))
            {
                connectionIds.Remove(connectionId);
            }
        }

        public ICollection<Guid> GetRoutes(WebSocketServerRouterAction action)
        {
            if (_listeners.TryGetValue(action.ActionName, out var parameterDicts)
                && parameterDicts.TryGetValue(action.Parameter, out var connectionIds))
                return connectionIds;

            return new HashSet<Guid>();
        }
    }
}