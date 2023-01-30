using Headlines.DTO.Entities;
using Headlines.WebAPI.Contracts.V1.Models;

namespace Headlines.WebAPI.Contracts.V1
{
    public sealed class MapperV1
    {
        public ArticleSourceModel MapArticleSource(ArticleSourceDTO articleSource)
        {
            return new ArticleSourceModel
            {
                Id = articleSource.Id,
                Name = articleSource.Name
            };
        }

        public ArticleModel MapArticle(ArticleDTO article)
        {
            return new ArticleModel
            {
                Id = article.Id,
                Published = article.Published,
                UrlId = article.UrlId,
                CurrentTitle = article.CurrentTitle,
                Link = article.Link
            };
        }

        public HeadlineChangeModel MapHeadlineChange(HeadlineChangeDTO headlineChange)
        {
            return new HeadlineChangeModel
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
}