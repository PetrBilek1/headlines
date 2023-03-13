using Headlines.Enums;

namespace Headlines.DTO.Entities
{
    public sealed class ArticleSourceDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string RssUrl { get; set; }
        public ArticleUrlIdSource UrlIdSource { get; set; }
        public ArticleScraperType? ScraperType { get; set; }
    }
}