using FluentAssertions;
using Headlines.BL.Abstractions.ArticleScraping;
using Headlines.BL.Implementations.ArticleScraper;
using Headlines.Enums;
using Moq;
using System.Reflection;
using Xunit;

namespace Headlines.BL.Tests.Implementations.ArticleScraper
{
    public sealed class ArticleScraperProviderTests
    {
        private readonly ArticleScraperProvider _sut;

        private readonly Mock<IHtmlDocumentLoader> _documentLoaderMock = new();

        public ArticleScraperProviderTests()
        {
            _sut = new ArticleScraperProvider(_documentLoaderMock.Object);
        }

        [Fact]
        public void Provide_ShouldReturnScraperForEveryScraperType()
        {
            var scraperTypes = Assembly
                .GetAssembly(typeof(IArticleScraper))!
                .GetTypes()
                .Where(x => x.IsClass && x.IsSubclassOf(typeof(IArticleScraper)))
                .ToList();

            var implementedScraperTypes = scraperTypes.Select(x => (Activator.CreateInstance(x, _documentLoaderMock.Object) as IArticleScraper)!.ScraperType);

            foreach(var type in implementedScraperTypes)
            {
                //Act
                IArticleScraper scraper = _sut.Provide(type);

                //Assert
                scraper.Should().NotBeNull();
                scraper.ScraperType.Should().Be(type);
            }
        }
    }
}