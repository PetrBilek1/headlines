using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Headlines.WebAPI.Contracts.V1.Requests.HeadlineChanges
{
    public sealed class UpvoteRequest
    {
        [Required]
        [JsonProperty("headlineChangeId")]
        public long HeadlineChangeId { get; set; }
        [Required]
        [JsonProperty("userToken")]
        public string UserToken { get; set; }
    }
}