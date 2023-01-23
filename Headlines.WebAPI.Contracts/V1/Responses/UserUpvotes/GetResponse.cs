using Headlines.WebAPI.Contracts.V1.Models;
using Newtonsoft.Json;

namespace Headlines.WebAPI.Contracts.V1.Responses.UserUpvotes
{
    public sealed class GetResponse
    {
        [JsonProperty("upvotes")]
        public List<UpvoteModel> Upvotes { get; set; }
    }
}