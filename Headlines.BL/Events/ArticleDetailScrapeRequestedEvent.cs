namespace Headlines.BL.Events
{
    public record ArticleDetailScrapeRequestedEvent
    {
        public long ArticleId { get; init; }
    }
}