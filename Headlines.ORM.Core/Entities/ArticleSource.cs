using Headlines.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PBilek.ORM.Core.Entity;

namespace Headlines.ORM.Core.Entities
{
    public sealed class ArticleSource : IEntity<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string RssUrl { get; set; }
        public ArticleUrlIdSource UrlIdSource { get; set; }
    }

    public sealed class HeadlineSourceConfiguration : IEntityTypeConfiguration<ArticleSource>
    {
        public void Configure(EntityTypeBuilder<ArticleSource> builder)
        {
            builder.ToTable("ARTICLE_SOURCE").HasKey(x => x.Id);

            builder.Property(x => x.Name).HasColumnName("NAME").HasMaxLength(256);
            builder.Property(x => x.RssUrl).HasColumnName("RSS_URL").HasMaxLength(512);
            builder.Property(x => x.UrlIdSource).HasColumnName("URL_ID_SOURCE");
        }
    }
}