using Headlines.WebAPI.Contracts.V1.Models;
using Newtonsoft.Json;

namespace Headlines.WebAPI.Contracts.V1.Responses.HeadlineChanges
{
    public sealed class GetSkipTakeResponse
    {
        [JsonProperty("headlineChanges")]
        public List<HeadlineChangeModel> HeadlineChanges { get; set; }
    }
}