using Newtonsoft.Json;

namespace Headlines.WebAPI.Contracts.V1.Models
{
    public sealed class ArticleDetailModel
    {
        [JsonProperty("isPaywalled")]
        public bool IsPaywalled { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;
        [JsonProperty("author")]
        public string Author { get; set; } = string.Empty;
        [JsonProperty("paragraphs")]
        public List<string> Paragraphs { get; set; } = new();
        [JsonProperty("tags")]
        public List<string> Tags { get; set; } = new();
    }
}