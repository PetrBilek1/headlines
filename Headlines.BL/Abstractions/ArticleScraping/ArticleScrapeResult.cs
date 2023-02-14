﻿namespace Headlines.BL.Abstractions.ArticleScraping
{
    public record ArticleScrapeResult
    {
        public bool IsSuccess { get; init; }
        public bool IsPaywalled { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Author { get; init; } = string.Empty;
        public string Content { get; init; } = string.Empty;
        public List<string> Tags { get; init; } = new();
    }
}