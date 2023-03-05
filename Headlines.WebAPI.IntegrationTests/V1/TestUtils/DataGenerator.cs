using Bogus;
using Bogus.DataSets;
using Headlines.DTO.Custom;
using Headlines.DTO.Entities;
using Headlines.Enums;
using Headlines.WebAPI.Contracts.V1.Models;

namespace Headlines.WebAPI.Tests.Integration.V1.TestUtils
{
    internal static class DataGenerator
    {
        public static int Seed { get; } = 6684796;

        public static IEnumerable<ArticleSourceDTO> GenerateArticleSources(int count)
        {
            var articleSourceGenerator = ArticleSourceDTO();

            for (int i = 0; i < count; i++)
            {
                yield return articleSourceGenerator.Generate();
            }
        }

        public static IEnumerable<ArticleDTO> GenerateArticles(int count, bool sourceHasScraper = false)
        {
            var articleSourceGenerator = ArticleSourceDTO();
            if (sourceHasScraper)
            {
                articleSourceGenerator.RuleFor(x => x.ScraperType, ArticleScraperType.Default);
            }

            var articleGenerator = ArticleDTO().RuleFor(x => x.Source, articleSourceGenerator);

            for (int i = 0; i < count; i++)
            {
                yield return articleGenerator.Generate();
            }
        }

        public static IEnumerable<ArticleDetailDTO> GenerateArticleDetails(int count)
        {
            var articleDetailGenerator = ArticleDetailDTO();

            for (int i = 0; i < count; i++)
            {
                yield return articleDetailGenerator.Generate();
            }
        }

        public static IEnumerable<HeadlineChangeDTO> GenerateHeadlineChanges(int count)
        {
            var articleSourceGenerator = ArticleSourceDTO();
            var articleGenerator = ArticleDTO().RuleFor(x => x.Source, articleSourceGenerator);
            var headlineChangeGenerator = HeadlineChangeDTO().RuleFor(x => x.Article, articleGenerator);

            for (int i = 0; i < count; i++)
            {
                yield return headlineChangeGenerator.Generate();
            }
        }

        public static IEnumerable<UpvoteModel> GenerateUserUpvotes(int count)
        {
            var usedTargetIds = new HashSet<long>();

            for (int i = 0; i < count; i++)
            {
                UpvoteModel upvote = UpvoteModel().Generate();
                while (usedTargetIds.Contains(upvote.TargetId))
                {
                    upvote = UpvoteModel().Generate();
                }

                usedTargetIds.Add(upvote.TargetId);
                yield return upvote;
            }
        }

        public static Faker<UpvoteModel> UpvoteModel()
        {
            return new Faker<UpvoteModel>()
                .UseSeed(Seed)
                .RuleFor(x => x.Type, y => UpvoteType.HeadlineChange)
                .RuleFor(x => x.TargetId, faker => faker.Random.Long(1, 9000))
                .RuleFor(x => x.Date, faker => faker.Date.Between(new DateTime(2020, 10, 1), new DateTime(2022, 10, 1)));
        }

        public static Faker<HeadlineChangeDTO> HeadlineChangeDTO()
        {
            return new Faker<HeadlineChangeDTO>()
                .UseSeed(Seed)
                .RuleFor(x => x.Id, y => default)
                .RuleFor(x => x.ArticleId, y => default)
                .RuleFor(x => x.Detected, faker => faker.Date.Between(new DateTime(2020, 10, 1), new DateTime(2022, 10, 1)))
                .RuleFor(x => x.TitleBefore, faker => faker.Lorem.Sentences(2, ""))
                .RuleFor(x => x.TitleAfter, faker => faker.Lorem.Sentences(2, ""))
                .RuleFor(x => x.UpvoteCount, faker => faker.Random.Long(0, 1000));
        }

        public static Faker<ArticleDTO> ArticleDTO(int detailCount = 0)
        {
            return new Faker<ArticleDTO>()
                .UseSeed(Seed)
                .RuleFor(x => x.Id, y => default)
                .RuleFor(x => x.SourceId, y => default)
                .RuleFor(x => x.Published, faker => faker.Date.Between(new DateTime(2020, 10, 1), new DateTime(2022, 10, 1)))
                .RuleFor(x => x.UrlId, y => Guid.NewGuid().ToString())
                .RuleFor(x => x.CurrentTitle, faker => faker.Lorem.Sentences(2, ""))
                .RuleFor(x => x.Link, y => Guid.NewGuid().ToString())
                .RuleFor(x => x.Details, y => ObjectDataDTO().GenerateBetween(detailCount, detailCount));
        }

        public static Faker<ArticleSourceDTO> ArticleSourceDTO()
        {
            return new Faker<ArticleSourceDTO>()
                .UseSeed(Seed)
                .RuleFor(x => x.Id, y => default)
                .RuleFor(x => x.Name, faker => faker.Lorem.Word())
                .RuleFor(x => x.RssUrl, y => Guid.NewGuid().ToString())
                .RuleFor(x => x.UrlIdSource, y => ArticleUrlIdSource.Id);
        }

        public static Faker<ObjectDataDTO> ObjectDataDTO()
        {
            return new Faker<ObjectDataDTO>()
                .UseSeed(Seed)
                .RuleFor(x => x.Id, y => default)
                .RuleFor(x => x.Bucket, y => "bucket")
                .RuleFor(x => x.Key, faker => $"{faker.Lorem.Word()}.json")
                .RuleFor(x => x.ContentType, y => "application/json")
                .RuleFor(x => x.Created, faker => faker.Date.Between(new DateTime(2020, 10, 1), new DateTime(2022, 10, 1)));
        }

        public static Faker<ArticleDetailDTO> ArticleDetailDTO()
        {
            return new Faker<ArticleDetailDTO>()
                .UseSeed(Seed)
                .RuleFor(x => x.IsPaywalled, faker => faker.Random.Bool())
                .RuleFor(x => x.Title, faker => faker.Lorem.Sentence())
                .RuleFor(x => x.Author, faker => faker.Name.FullName())
                .RuleFor(x => x.Paragraphs, faker => faker.Make(5, y => faker.Lorem.Sentences(5)))
                .RuleFor(x => x.Tags, faker => faker.Make(4, y => faker.Lorem.Word()));
        }
    }
}