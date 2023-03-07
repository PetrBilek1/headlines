using Headlines.WebAPI.Contracts.V1.Models;
using Newtonsoft.Json;

namespace Headlines.WebAPI.Contracts.V1.Responses.Articles
{
    public sealed class GetHeadlineChangesByArticleIdResponse
    {
        [JsonProperty("headlineChanges")]
        public List<HeadlineChangeModel> HeadlineChanges { get; set; }
        [JsonProperty("totalCount")]
        public long TotalCount { get; set; }
    }
}