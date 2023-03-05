using Newtonsoft.Json;

namespace Headlines.WebAPI.Contracts.WebSockets
{
    public sealed class ListenToActionMessage
    {
        [JsonProperty("actionName")]
        public string ActionName { get; set; }
        [JsonProperty("parameter")]
        public string Parameter { get; set; }
    }
}