using Headlines.DTO.Entities;
using Headlines.Enums;
using PBilek.RSSReaderService;

namespace Headlines.RSSProcessingMicroService.Utils
{
    public static class FeedItemUtils
    {
        public static string GetUrlId(FeedItemDTO feedItem, ArticleSourceDTO source)
        {
            return source.UrlIdSource switch
            {
                ArticleUrlIdSource.Id => feedItem.Id,
                ArticleUrlIdSource.Link => feedItem.Link,
                _ => throw new NotImplementedException($"{nameof(source.UrlIdSource)} is not implemented."),
            };
        }
    }
}