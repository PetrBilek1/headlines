using Headlines.DTO.Attributes;

namespace Headlines.DTO.Custom
{
    [ObjectStorageName("article-detail")]
    public sealed class ArticleDetailDto
    {
        public bool IsPaywalled { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public List<string> Paragraphs { get; set; } = new();
        public List<string> Tags { get; set; } = new();
    }
}