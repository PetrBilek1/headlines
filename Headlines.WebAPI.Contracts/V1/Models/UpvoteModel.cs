using Headlines.Enums;
using Newtonsoft.Json;

namespace Headlines.WebAPI.Contracts.V1.Models
{
    public class UpvoteModel
    {
        [JsonProperty("type")]
        public UpvoteType Type { get; set; }
        [JsonProperty("targetId")]
        public long TargetId { get; set; }
        [JsonProperty("date")]
        public DateTime Date { get; set; }
    }
}