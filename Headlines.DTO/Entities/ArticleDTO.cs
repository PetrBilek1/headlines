namespace Headlines.DTO.Entities
{
    public sealed class ArticleDto
    {
        public long Id { get; set; }
        public long SourceId { get; set; }

        public DateTime? Published { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Changed { get; set; }
        public string UrlId { get; set; }
        public string CurrentTitle { get; set; }
        public string Link { get; set; }

        public ArticleSourceDto Source { get; set; }
        public List<ObjectDataDto> Details { get; set; }
    }
}