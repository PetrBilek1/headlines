using Headlines.WebAPI.Contracts.V1.Models;
using Newtonsoft.Json;

namespace Headlines.WebAPI.Contracts.V1.Responses.Articles
{
    public sealed class GetArticlesSkipTakeResponse
    {
        [JsonProperty("articles")]
        public List<ArticleModel> Articles { get; set; }
        [JsonProperty("matchesFiltersCount")]
        public long MatchesFiltersCount { get; set; }
    }
}