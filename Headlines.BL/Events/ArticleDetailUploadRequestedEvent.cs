using Headlines.DTO.Custom;

namespace Headlines.BL.Events
{
    public record ArticleDetailUploadRequestedEvent
    {
        public long ArticleId { get; init; }
        public ArticleDetailDto Detail { get; init; } = new ArticleDetailDto();
    }
}