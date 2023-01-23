﻿using Newtonsoft.Json;

namespace Headlines.WebAPI.Contracts.V1.Models
{
    public sealed class ArticleModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("published")]
        public DateTime? Published { get; set; }
        [JsonProperty("urlId")]
        public string UrlId { get; set; }
        [JsonProperty("currentTitle")]
        public string CurrentTitle { get; set; }
        [JsonProperty("link")]
        public string Link { get; set; }
    }
}