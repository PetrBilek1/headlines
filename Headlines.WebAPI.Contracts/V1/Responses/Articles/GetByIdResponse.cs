using Headlines.WebAPI.Contracts.V1.Models;
using Newtonsoft.Json;

namespace Headlines.WebAPI.Contracts.V1.Responses.Articles
{
    public sealed class GetByIdResponse
    {
        [JsonProperty("article")]
        public ArticleModel Article { get; set; }
    }
}