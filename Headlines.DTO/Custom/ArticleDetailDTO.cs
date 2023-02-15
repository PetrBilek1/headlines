using Headlines.DTO.Attributes;

namespace Headlines.DTO.Custom
{
    [ObjectStorageName("article-detail")]
    public sealed class ArticleDetailDTO
    {
        public bool IsPaywalled { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
    }
}