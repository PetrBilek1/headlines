using Headlines.DTO.Custom;

namespace Headlines.BL.Events
{
    public record ArticleDetailScrapeResultEvent
    {
        public long ArticleId { get; init; }
        public bool WasSuccessful { get; init; }
        public ArticleDetailDto Detail { get; init; } = new();
    }
}