using Newtonsoft.Json;

namespace Headlines.WebAPI.Contracts.V1.Models
{
    public sealed class HeadlineChangeModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("articleId")]
        public long ArticleId { get; set; }

        [JsonProperty("detected")]
        public DateTime Detected { get; set; }
        [JsonProperty("titleBefore")]
        public string TitleBefore { get; set; }
        [JsonProperty("titleAfter")]
        public string TitleAfter { get; set; }
        [JsonProperty("upvoteCount")]
        public long UpvoteCount { get; set; }

        [JsonProperty("article")]
        public ArticleModel Article { get; set; }
    }
}