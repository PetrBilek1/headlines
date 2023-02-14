namespace Headlines.BL.Abstractions.ArticleScraping
{
    public record ArticleScrapeResult
    {
        public bool IsSuccess { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}