using Headlines.WebAPI.Contracts.V1.Models;
using Newtonsoft.Json;

namespace Headlines.WebAPI.Contracts.V1.Responses.ArticleSources
{
    public sealed class GetAllResponse
    {
        [JsonProperty("articleSources")]
        public List<ArticleSourceModel> ArticleSources { get; set; }
    }
}