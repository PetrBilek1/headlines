using Headlines.RSSProcessingMicroService.DTO;

namespace Headlines.RSSProcessingMicroService.Services
{
    public interface IRSSProcessorService
    {
        Task<ProcessingResultDTO> DoWorkAsync(CancellationToken cancellationToken = default);
    }
}