namespace Headlines.BL.Events
{
    public record ArticleDetailScrapeRequestedEvent
    {
        public long ArticleId { get; init; }
        public long Retried { get; init; }
    }
}