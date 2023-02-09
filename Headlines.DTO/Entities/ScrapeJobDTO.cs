using Headlines.Enums;

namespace Headlines.DTO.Entities
{
    public sealed class ScrapeJobDTO
    {
        public long Id { get; set; }
        public long ArticleId { get; set; }
        public ScrapeJobPriority Priority { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Changed { get; set; }

        public ArticleDTO Article { get; set; }
    }
}