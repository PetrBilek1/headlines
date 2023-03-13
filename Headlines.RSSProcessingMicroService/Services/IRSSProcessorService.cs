using Headlines.RSSProcessingMicroService.DTO;

namespace Headlines.RSSProcessingMicroService.Services
{
    public interface IRssProcessorService
    {
        Task<ProcessingResultDto> DoWorkAsync(CancellationToken cancellationToken = default);
    }
}