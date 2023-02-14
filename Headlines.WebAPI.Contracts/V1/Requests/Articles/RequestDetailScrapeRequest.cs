using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Headlines.WebAPI.Contracts.V1.Requests.Articles
{
    public sealed class RequestDetailScrapeRequest
    {
        [Required]
        [JsonProperty("articleId")]
        public long ArticleId { get; set; }
    }
}