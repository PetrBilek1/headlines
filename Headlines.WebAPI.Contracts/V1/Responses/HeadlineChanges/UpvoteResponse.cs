using Headlines.WebAPI.Contracts.V1.Models;
using Newtonsoft.Json;

namespace Headlines.WebAPI.Contracts.V1.Responses.HeadlineChanges
{
    public sealed class UpvoteResponse
    {
        [JsonProperty("upvotes")]
        public List<UpvoteModel> Upvotes { get; set; }
    }
}