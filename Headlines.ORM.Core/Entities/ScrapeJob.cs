using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PBilek.ORM.Core.Entity;
using Headlines.Enums;

namespace Headlines.ORM.Core.Entities
{
    public sealed class ScrapeJob : IEntity<long>, IRecordEntity
    {
        public long Id { get; set; }
        public long ArticleId { get; set; }
        public ScrapeJobPriority Priority { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Changed { get; set; }

        public Article Article { get; set; }
    }

    public sealed class ScrapeJobConfiguration : IEntityTypeConfiguration<ScrapeJob>
    {
        public void Configure(EntityTypeBuilder<ScrapeJob> builder)
        {
            builder.ToTable("SCRAPE_JOB").HasKey(x => x.Id);

            builder.Property(x => x.ArticleId).HasColumnName("ARTICLE_ID");
            builder.Property(x => x.Priority).HasColumnName("PRIORITY");

            builder.HasOne(x => x.Article).WithMany().HasForeignKey(x => x.ArticleId);
        }
    }
}