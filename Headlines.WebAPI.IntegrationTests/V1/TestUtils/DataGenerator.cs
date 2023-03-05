using Bogus;
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

        public static IEnumerable<ArticleDTO> GenerateArticles(int count)
        {
            var articleSourceGenerator = ArticleSourceDTO();
            var articleGenerator = ArticleDTO().RuleFor(x => x.Source, articleSourceGenerator);

            for (int i = 0; i < count; i++)
            {
                yield return articleGenerator.Generate();
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

        public static Faker<ArticleDTO> ArticleDTO()
        {
            return new Faker<ArticleDTO>()
                .UseSeed(Seed)
                .RuleFor(x => x.Id, y => default)
                .RuleFor(x => x.SourceId, y => default)
                .RuleFor(x => x.Published, faker => faker.Date.Between(new DateTime(2020, 10, 1), new DateTime(2022, 10, 1)))
                .RuleFor(x => x.UrlId, y => Guid.NewGuid().ToString())
                .RuleFor(x => x.CurrentTitle, faker => faker.Lorem.Sentences(2, ""))
                .RuleFor(x => x.Link, y => Guid.NewGuid().ToString());
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
    }
}