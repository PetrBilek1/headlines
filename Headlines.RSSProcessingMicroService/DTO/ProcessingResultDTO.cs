using Headlines.DTO.Entities;

namespace Headlines.RSSProcessingMicroService.DTO
{
    public sealed class ProcessingResultDto
    {
        public List<ArticleDto> CreatedArticles { get; set; } = new List<ArticleDto>();
        public List<ArticleDto> UpdatedArticles { get; set; } = new List<ArticleDto>();
        public List<HeadlineChangeDto> RecordedHeadlineChanges { get; set; } = new List<HeadlineChangeDto>();
    }
}