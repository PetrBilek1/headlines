namespace Headlines.WebAPI.Middlewares.WebSocketServer.Abstraction
{
    public static class WebSocketServerRouterActions
    {
        public static class ListenerKeys
        {
            public static string ArticleDetailScraped(long articleId) => $"action:{Names.ArticleDetailScraped}:{articleId}";
        }

        public static class Names
        {
            public const string ArticleDetailScraped = "article-detail-scraped";
        }
    }
}