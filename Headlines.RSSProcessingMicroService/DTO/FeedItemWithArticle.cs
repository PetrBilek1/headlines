using Headlines.DTO.Entities;
using PBilek.RSSReaderService;

namespace Headlines.RSSProcessingMicroService.DTO
{
    public sealed class FeedItemWithArticle
    {        
        public FeedItemDTO? FeedItem { get; set; }
        public ArticleSourceDto? ArticleSource { get; set; }
        public ArticleDto? Article { get; set; }
    }
}