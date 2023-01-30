using Newtonsoft.Json;

namespace Headlines.WebAPI.Contracts.V1.Models
{
    public sealed class ArticleSourceModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}