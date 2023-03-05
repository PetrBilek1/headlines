using Headlines.Enums;

namespace Headlines.BL.Abstractions.ArticleScraping
{
    public interface IArticleScraperProvider
    {
        IArticleScraper Provide(ArticleScraperType scraperType);
    }
}