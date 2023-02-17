namespace Headlines.BL.Abstractions.ArticleScraping
{
    public abstract class ArticleScraperBase : IArticleScraper
    {
        protected IHtmlDocumentLoader _documentLoader;

        protected ArticleScraperBase(IHtmlDocumentLoader documentLoader) 
        { 
            _documentLoader = documentLoader;
        }

        public abstract Task<ArticleScrapeResult> ScrapeArticleAsync(string url);
    }
}