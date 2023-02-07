using Headlines.WebAPI.Contracts.V1.Models;
using Newtonsoft.Json;

namespace Headlines.WebAPI.Contracts.V1.Responses.HeadlineChanges
{
    public sealed class GetByArticleIdSkipTakeResponse
    {
        [JsonProperty("headlineChanges")]
        public List<HeadlineChangeModel> HeadlineChanges { get; set; }
        [JsonProperty("totalCount")]
        public long TotalCount { get; set; }
    }
}