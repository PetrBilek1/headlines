namespace Headlines.DTO.Entities
{
    public sealed class ArticleDTO
    {
        public long Id { get; set; }
        public long SourceId { get; set; }

        public DateTime? Published { get; set; }
        public DateTime? Updated { get; set; }
        public string UrlId { get; set; }
        public string CurrentTitle { get; set; }
        public string Link { get; set; }

        public ArticleSourceDTO Source { get; set; }
    }
}