using Headlines.WebAPI.Contracts.V1.Models;
using Newtonsoft.Json;

namespace Headlines.WebAPI.Contracts.WebSockets
{
    public sealed class ArticleDetailScrapedMessage
    {
        [JsonProperty("messageType")]
        public const string MessageType = "article-detail-scraped";
        [JsonProperty("articleId")]
        public long ArticleId { get; set; }
        [JsonProperty("detail")]
        public ArticleDetailModel Detail { get; set; }
    }
}