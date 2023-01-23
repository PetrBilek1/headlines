using Headlines.DTO.Entities;

namespace Headlines.RSSProcessingMicroService.DTO
{
    public sealed class ProcessingResultDTO
    {
        public List<ArticleDTO> CreatedArticles { get; set; } = new List<ArticleDTO>();
        public List<ArticleDTO> UpdatedArticles { get; set; } = new List<ArticleDTO>();
        public List<HeadlineChangeDTO> RecordedHeadlineChanges { get; set; } = new List<HeadlineChangeDTO>();
    }
}