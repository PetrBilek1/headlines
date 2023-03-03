using Headlines.WebAPI.Contracts.V1.Models;
using Newtonsoft.Json;

namespace Headlines.WebAPI.Contracts.V1.Responses.Articles
{
    public sealed class GetDetailByIdResponse
    {
        [JsonProperty("detail")]
        public ArticleDetailModel Detail { get; set; }
    }
}