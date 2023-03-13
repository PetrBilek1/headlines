namespace Headlines.DTO.Entities
{
    public sealed class HeadlineChangeDto
    {
        public long Id { get; set; }
        public long ArticleId { get; set; }

        public DateTime Detected { get; set; }
        public string TitleBefore { get; set; }
        public string TitleAfter { get; set; }
        public long UpvoteCount { get; set; }

        public ArticleDto Article { get; set; }
    }
}