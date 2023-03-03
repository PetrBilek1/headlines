using Headlines.BL.Events;
using Headlines.WebAPI.Contracts.V1;
using Headlines.WebAPI.Contracts.WebSockets;
using Headlines.WebAPI.Middlewares.WebSocketServer.Abstraction;
using MassTransit;
using Newtonsoft.Json;

namespace Headlines.WebAPI.Consumers
{
    public sealed class ArticleDetailScrapeResultEventConsumer : IConsumer<ArticleDetailScrapeResultEvent>
    {
        private readonly IWebSocketServerService _server;
        private readonly MapperV1 _mapper = new();

        public ArticleDetailScrapeResultEventConsumer(IWebSocketServerService server)
        {
            _server = server;
        }

        public async Task Consume(ConsumeContext<ArticleDetailScrapeResultEvent> context)
        {
            await _server.SendMessageByActionAsync(
                WebSocketServerRouterAction.ArticleDetailScraped.Create(context.Message.ArticleId), 
                JsonConvert.SerializeObject(new ArticleDetailScrapedMessage
                {
                    ArticleId = context.Message.ArticleId,
                    Detail = _mapper.MapArticleDetail(context.Message.Detail)
                })
            );
        }
    }
}