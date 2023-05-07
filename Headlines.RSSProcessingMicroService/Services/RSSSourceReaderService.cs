using Headlines.BL.Facades;
using Headlines.DTO.Entities;
using Headlines.RSSProcessingMicroService.DTO;
using Headlines.RSSProcessingMicroService.Utils;
using PBilek.RSSReaderService;

namespace Headlines.RSSProcessingMicroService.Services
{
    public sealed class RssSourceReaderService : IRssSourceReaderService
    {
        private readonly IRSSReaderService _rssReaderService;
        private readonly IArticleSourceFacade _articleSourceFacade;
        private readonly IArticleFacade _articleFacade;
        private readonly ILogger<RssSourceReaderService> _logger;

        public RssSourceReaderService(IRSSReaderService rssReaderService, IArticleSourceFacade articleSourceFacade, IArticleFacade articleFacade, ILogger<RssSourceReaderService> logger)
        {
            _rssReaderService = rssReaderService;
            _articleSourceFacade = articleSourceFacade;
            _articleFacade = articleFacade;
            _logger = logger;
        }

        public async Task<List<FeedItemWithArticle>> ReadFeedItemsFromSourcesAsync(CancellationToken cancellationToken = default)
        {
            List<ArticleSourceDto> sources = await _articleSourceFacade.GetAllArticleSourcesAsync(cancellationToken);

            var feedItems = new List<FeedItemWithArticle>();

            foreach(ArticleSourceDto source in sources)
            {
                feedItems.AddRange(await ReadFeedItemsFromSourceAsync(source, cancellationToken));
            }

            return feedItems;
        }

        private async Task<List<FeedItemWithArticle>> ReadFeedItemsFromSourceAsync(ArticleSourceDto source, CancellationToken cancellationToken)
        {
            var feedItems = new List<FeedItemDTO>();

            try
            {
                feedItems = await _rssReaderService.ReadFeedItemsAsync(source.RssUrl, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Could not read RSS feed of source '{name}' with exception '{message}'.", source.Name, e.Message);
                return new List<FeedItemWithArticle>();
            }

            string[] urlIds = feedItems.Select(x => FeedItemUtils.GetUrlId(x, source)).ToArray();
            List<ArticleDto> articles = await _articleFacade.GetArticlesByUrlIdsAsync(urlIds, cancellationToken);
            Dictionary<string, ArticleDto> articlesByUrlId = articles.Where(x => x.SourceId == source.Id).ToDictionary(x => x.UrlId);

            return feedItems.Select(feedItem => new FeedItemWithArticle()
            {
                FeedItem = feedItem,
                ArticleSource = source,
                Article = articlesByUrlId.TryGetValue(FeedItemUtils.GetUrlId(feedItem, source), out ArticleDto? outArticle)
                        ? outArticle
                        : null
            }).ToList();
        }      
    }    
}