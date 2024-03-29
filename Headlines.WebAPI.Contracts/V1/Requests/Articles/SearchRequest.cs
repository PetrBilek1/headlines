﻿using Newtonsoft.Json;

namespace Headlines.WebAPI.Contracts.V1.Requests.Articles
{
    public sealed class SearchRequest
    {
        [JsonProperty("skip")]
        public int? Skip { get; set; }
        [JsonProperty("take")]
        public int? Take { get; set; }
        [JsonProperty("searchPrompt")]
        public string SearchPrompt { get; set; }
        [JsonProperty("articleSources")]
        public long[] ArticleSources { get; set; }
        [JsonProperty("publishedUtcFrom")]
        public DateTime? PublishedUtcFrom { get; set; }
        [JsonProperty("publishedUtcTo")]
        public DateTime? PublishedUtcTo { get; set; }
    }
}