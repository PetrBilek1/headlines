using Headlines.DTO.Custom;
using Headlines.DTO.Entities;
using Headlines.WebAPI.Contracts.V1.Models;

namespace Headlines.WebAPI.Contracts.V1
{
    public sealed class MapperV1
    {
        public ArticleSourceModel MapArticleSource(ArticleSourceDTO articleSource)
            => new ArticleSourceModel
            {
                Id = articleSource.Id,
                Name = articleSource.Name,
                ScrapingSupported = articleSource.ScraperType.HasValue
            };

        public ArticleModel MapArticle(ArticleDTO article)
            => new ArticleModel
            {
                Id = article.Id,
                SourceId = article.SourceId,
                Published = article.Published,
                UrlId = article.UrlId,
                CurrentTitle = article.CurrentTitle,
                Link = article.Link,
                Source = article.Source == null
                    ? null 
                    : MapArticleSource(article.Source)
            };

        public ArticleDetailModel MapArticleDetail(ArticleDetailDTO articleDetail)
            => new ArticleDetailModel
            {
                IsPaywalled = articleDetail.IsPaywalled,
                Title = articleDetail.Title,
                Author = articleDetail.Author,
                Paragraphs = articleDetail.Paragraphs,
                Tags = articleDetail.Tags,
            };

        public HeadlineChangeModel MapHeadlineChange(HeadlineChangeDTO headlineChange)
            => new HeadlineChangeModel
            {
                Id = headlineChange.Id,
                ArticleId = headlineChange.ArticleId,
                Detected = headlineChange.Detected,
                TitleBefore = headlineChange.TitleBefore,
                TitleAfter = headlineChange.TitleAfter,
                UpvoteCount = headlineChange.UpvoteCount,
                Article = headlineChange.Article == null
                    ? null
                    : MapArticle(headlineChange.Article)
            };
    }
}