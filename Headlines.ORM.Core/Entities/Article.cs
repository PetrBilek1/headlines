using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PBilek.ORM.Core.Entity;

namespace Headlines.ORM.Core.Entities
{
    public sealed class Article : IEntity<long>, IRecordEntity
    {
        public long Id { get; set; }

        public long SourceId { get; set; }

        public DateTime? Published { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Changed { get; set; }
        public string UrlId { get; set; }
        public string CurrentTitle { get; set; }
        public string Link { get; set; }

        public ArticleSource Source { get; set; }        
    }

    public sealed class ArticleConfiguration : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> builder)
        {
            builder.ToTable("ARTICLE").HasKey(x => x.Id);

            builder.Property(x => x.SourceId).HasColumnName("SOURCE_ID");
            builder.Property(x => x.Published).HasColumnName("PUBLISHED");
            builder.Property(x => x.Created).HasColumnName("CREATED");
            builder.Property(x => x.Changed).HasColumnName("CHANGED");
            builder.Property(x => x.UrlId).HasColumnName("URL_ID").HasMaxLength(1024);
            builder.Property(x => x.CurrentTitle).HasColumnName("CURRENT_TITLE").HasMaxLength(1024);
            builder.Property(x => x.Link).HasColumnName("LINK").HasMaxLength(512);

            builder.HasIndex(x => x.Published);
            builder.HasIndex(x => x.UrlId);

            builder.HasOne(x => x.Source).WithMany().HasForeignKey(x => x.SourceId);
        }
    }
}