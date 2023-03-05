using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PBilek.ORM.Core.Entity;

namespace Headlines.ORM.Core.Entities
{
    public sealed class ObjectData : IEntity<long>, IRecordEntity
    {
        public long Id { get; set; }

        public string Bucket { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;

        public DateTime? Created { get; set; }
        public DateTime? Changed { get; set; }
    }

    public sealed class ObjectDataConfiguration : IEntityTypeConfiguration<ObjectData>
    {
        public void Configure(EntityTypeBuilder<ObjectData> builder)
        {
            builder.ToTable("OBJECT_DATA").HasKey(x => x.Id);

            builder.Property(x => x.Bucket).HasColumnName("BUCKET").HasMaxLength(256);
            builder.Property(x => x.Key).HasColumnName("KEY").HasMaxLength(512);
            builder.Property(x => x.ContentType).HasColumnName("CONTENT_TYPE");
        }
    }
}
