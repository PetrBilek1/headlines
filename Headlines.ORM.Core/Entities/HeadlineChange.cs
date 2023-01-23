using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PBilek.ORM.Core.Entity;

namespace Headlines.ORM.Core.Entities
{
    public sealed class HeadlineChange : IEntity<long>
    {
        public long Id { get; set; }
        public long ArticleId { get; set; }

        public DateTime Detected { get; set; }
        public string TitleBefore { get; set; }
        public string TitleAfter { get; set; }
        public long UpvoteCount { get; set; }

        public Article Article { get; set; }
    }

    public sealed class HeadlineChangeConfiguration : IEntityTypeConfiguration<HeadlineChange>
    {
        public void Configure(EntityTypeBuilder<HeadlineChange> builder)
        {
            builder.ToTable("HEADLINE_CHANGE").HasKey(x => x.Id);

            builder.Property(x => x.ArticleId).HasColumnName("ARTICLE_ID");
            builder.Property(x => x.Detected).HasColumnName("DETECTED");
            builder.Property(x => x.TitleBefore).HasColumnName("TITLE_BEFORE").HasMaxLength(1024);
            builder.Property(x => x.TitleAfter).HasColumnName("TITLE_AFTER").HasMaxLength(1024);
            builder.Property(x => x.UpvoteCount).HasColumnName("UPVOTE_COUNT");

            builder.HasIndex(x => x.Detected);
            builder.HasIndex(x => x.UpvoteCount);

            builder.HasOne(x => x.Article).WithMany().HasForeignKey(x => x.ArticleId);
        }
    }
}