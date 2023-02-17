using FluentAssertions;
using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.BL.Implementations.ArticleScraper;
using HtmlAgilityPack;
using Moq;

namespace Headlines.BL.Tests.Implementations.ArticleScraper
{
    public abstract class ScraperTestBase<TScraper>
        where TScraper : ArticleScraperBase, IArticleScraper
    {
        protected readonly TScraper _sut;

        private readonly Mock<IHtmlDocumentLoader> _documentLoaderMock = new Mock<IHtmlDocumentLoader>(MockBehavior.Strict);
        private readonly IHtmlDocumentSanitizer _sanitizer = new HtmlDocumentSanitizer();

        protected ScraperTestBase() 
        {
            _sut = Activator.CreateInstance(typeof(TScraper), _documentLoaderMock.Object) as TScraper ?? throw new Exception();
        }

        protected void SetupDocumentLoader(string html)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);

            document = _sanitizer.Sanitize(document);

            _documentLoaderMock.Setup(x => x.LoadFromUrlAsync(It.IsAny<string>()))
                .ReturnsAsync(document);
        }

        protected void AssertResult(ArticleScrapeResult actual, ArticleScrapeResult expected)
        {
            actual.Should().NotBeNull();
            actual.IsSuccess.Should().Be(expected.IsSuccess);
            actual.IsPaywalled.Should().Be(expected.IsPaywalled);
            actual.Title.Should().Be(expected.Title);
            actual.Author.Should().Be(expected.Author);

            actual.Paragraphs.Should().HaveCount(expected.Paragraphs.Count);
            for (int i = 0; i < expected.Paragraphs.Count; i++)
            {
                actual.Paragraphs[i].Should().Be(expected.Paragraphs[i]);
            }

            actual.Tags.Should().HaveCount(expected.Tags.Count);
            for (int i = 0; i < expected.Tags.Count; i++)
            {
                actual.Tags[i].Should().Be(expected.Tags[i]);
            }
        }
    }
}