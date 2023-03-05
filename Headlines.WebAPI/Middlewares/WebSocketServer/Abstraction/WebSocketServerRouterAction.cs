namespace Headlines.WebAPI.Middlewares.WebSocketServer.Abstraction
{
    public sealed class WebSocketServerRouterAction
    {
        public string ActionName { get; private set; } = string.Empty;
        public string Parameter { get; private set; } = string.Empty;
        public string ListenerKey => string.IsNullOrEmpty(Parameter) 
            ? $"action:{ActionName}" 
            : $"action:{ActionName}:{Parameter}";

        private WebSocketServerRouterAction() { }

        public static class ArticleDetailScraped
        {
            public const string ActionName = "article-detail-scraped";

            public static WebSocketServerRouterAction Create(long? articleId = null)
            => new WebSocketServerRouterAction
            {
                ActionName = ActionName,
                Parameter = articleId?.ToString() ?? string.Empty,
            };
        }

        public static class HeadlineChangeDetected
        {
            public const string ActionName = "headline-change-detected";

            public static WebSocketServerRouterAction Create(long? articleId = null)
            => new WebSocketServerRouterAction
            {
                ActionName = ActionName,
                Parameter = articleId?.ToString() ?? string.Empty,
            };
        }
    }
}