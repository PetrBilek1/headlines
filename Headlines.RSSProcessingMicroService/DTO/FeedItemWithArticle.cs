using Headlines.DTO.Entities;
using PBilek.RSSReaderService;

namespace Headlines.RSSProcessingMicroService.DTO
{
    public sealed class FeedItemWithArticle
    {        
        public FeedItemDTO? FeedItem { get; set; }
        public ArticleSourceDTO? ArticleSource { get; set; }
        public ArticleDTO? Article { get; set; }
    }
}