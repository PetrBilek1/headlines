using Headlines.WebAPI.Middlewares.WebSocketServer.Abstraction;
using Headlines.WebAPI.Middlewares.WebSocketServer.Implementation;

namespace Headlines.WebAPI.Middlewares.WebSocketServer
{
    public static class WebSocketServerMiddlewareExtensions
    {
        public static IApplicationBuilder UseWebSocketServer(this IApplicationBuilder app)
        {
            return app.UseMiddleware<WebSocketServerMiddleware>();
        }

        public static IServiceCollection AddWebSocketServerDependencyGroup(this IServiceCollection services)
        {
            services.AddSingleton<IWebSocketServerConnectionManager, WebSocketServerConnectionManager>();
            services.AddSingleton<IWebSocketServerRouter, WebSocketServerRouter>();
            services.AddSingleton<IWebSocketServerService, WebSocketServerService>();

            return services;
        }
    }
}