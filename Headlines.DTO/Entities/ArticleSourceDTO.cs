using Headlines.Enums;

namespace Headlines.DTO.Entities
{
    public sealed class ArticleSourceDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string RssUrl { get; set; }
        public ArticleUrlIdSource UrlIdSource { get; set; }
    }
}