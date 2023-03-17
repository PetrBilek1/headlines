using FluentAssertions;
using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.BL.Events;
using Headlines.BL.Facades;
using Headlines.DTO.Entities;
using Headlines.Enums;
using Headlines.ScrapeMicroService.Consumers;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Headlines.ScrapeMicroService.Tests.Consumers
{
    public sealed class ArticleDetailScrapeRequestedEventConsumerTests
    {
        private readonly ArticleDetailScrapeRequestedEventConsumer _sut;

        private readonly Mock<ConsumeContext<ArticleDetailScrapeRequestedEvent>> _consumeContextMock = new Mock<ConsumeContext<ArticleDetailScrapeRequestedEvent>>();
        private readonly Mock<MessageSchedulerContext> _messageSchedulerMock = new(MockBehavior.Strict);

        private readonly Mock<IArticleFacade> _articleFacadeMock = new(MockBehavior.Strict);
        private readonly Mock<IArticleScraperProvider> _scraperProviderMock = new(MockBehavior.Strict);
        private readonly Mock<IArticleScraper> _scraperMock = new(MockBehavior.Strict);
        private readonly Mock<ILogger<ArticleDetailScrapeRequestedEventConsumer>> _loggerMock = new Mock<ILogger<ArticleDetailScrapeRequestedEventConsumer>>();

        public ArticleDetailScrapeRequestedEventConsumerTests()
        {
            _sut = new ArticleDetailScrapeRequestedEventConsumer(_articleFacadeMock.Object, _scraperProviderMock.Object, _loggerMock.Object);
        }

        [Theory]
        [InlineData(true, 0)]
        [InlineData(false, 0)]
        [InlineData(false, 1)]
        public async Task Consume_Simple(bool scrapingSuccessful, int retried)
        {
            //Arrange
            long articleId = 10;
            string link = "link";
            string sourceName = "sourceName";

            _articleFacadeMock.Setup(x => x.GetArticleByIdIncludeSourceAsync(articleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ArticleDto
                {
                    Id = articleId,
                    Link = link,
                    Source = new ArticleSourceDto { Id = 1, Name = sourceName, ScraperType = ArticleScraperType.Default }
                });

            _scraperMock.Setup(x => x.ScrapeArticleAsync(link))
                .ReturnsAsync(new ArticleScrapeResult
                {
                    IsSuccess = scrapingSuccessful,
                });

            _scraperProviderMock.Setup(x => x.Provide(ArticleScraperType.Default))
                .Returns(_scraperMock.Object);

            _consumeContextMock.Setup(x => x.Message)
                .Returns(new ArticleDetailScrapeRequestedEvent
                {
                    ArticleId = articleId,
                    Retried = retried
                });

            _messageSchedulerMock.Setup(x => x.SchedulePublish(It.IsAny<DateTime>(), It.IsAny<ArticleDetailScrapeRequestedEvent>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((null as ScheduledMessage<ArticleDetailScrapeRequestedEvent>)!);

            var returnedScheduler = _messageSchedulerMock.Object;
            _consumeContextMock.Setup(x => x.TryGetPayload(out returnedScheduler))
                .Returns(true);

            _consumeContextMock.Setup(x => x.Publish(It.IsAny<ArticleDetailScrapeResultEvent>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _consumeContextMock.Setup(x => x.Publish(It.IsAny<ArticleDetailUploadRequestedEvent>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            if (retried < 1)
            {
                //Act
                await _sut.Consume(_consumeContextMock.Object);

                //Assert
                _articleFacadeMock.Verify(x => x.GetArticleByIdIncludeSourceAsync(articleId, It.IsAny<CancellationToken>()), Times.Once);
                _scraperProviderMock.Verify(x => x.Provide(ArticleScraperType.Default), Times.Once);
                _scraperMock.Verify(x => x.ScrapeArticleAsync(link), Times.Once);
                _consumeContextMock.Verify(x => x.Publish(It.IsAny<ArticleDetailScrapeResultEvent>(), It.IsAny<CancellationToken>()), scrapingSuccessful ? Times.Once : Times.Never);
                _consumeContextMock.Verify(x => x.Publish(It.IsAny<ArticleDetailUploadRequestedEvent>(), It.IsAny<CancellationToken>()), scrapingSuccessful ? Times.Once : Times.Never);
                _messageSchedulerMock.Verify(x => x.SchedulePublish(It.IsAny<DateTime>(), It.IsAny<ArticleDetailScrapeRequestedEvent>(), It.IsAny<CancellationToken>()), scrapingSuccessful ? Times.Never : Times.Once);
            }
            else
            {
                //Act
                Func<Task> act = async () => await _sut.Consume(_consumeContextMock.Object);

                //Assert
                await act.Should().ThrowAsync<ConsumerException>().WithMessage($"Scraping of article with Id '{articleId}' source Name '{sourceName}' was not successful '{retried + 1}' times.");
            }
        }
    }
}