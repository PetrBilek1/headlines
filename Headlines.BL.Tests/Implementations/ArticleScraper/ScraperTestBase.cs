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
    }
}