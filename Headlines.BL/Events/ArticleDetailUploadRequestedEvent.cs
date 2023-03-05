using Headlines.DTO.Custom;

namespace Headlines.BL.Events
{
    public record ArticleDetailUploadRequestedEvent
    {
        public long ArticleId { get; init; }
        public ArticleDetailDTO Detail { get; init; } = new ArticleDetailDTO();
    }
}