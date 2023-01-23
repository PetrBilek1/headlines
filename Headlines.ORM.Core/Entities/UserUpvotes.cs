using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PBilek.ORM.Core.Entity;

namespace Headlines.ORM.Core.Entities
{
    public sealed class UserUpvotes : IEntity<long>
    {
        public long Id { get; set; }
        public string UserToken { get; set; }
        public string Json { get; set; }
    }

    public sealed class UserUpvotesConfiguration : IEntityTypeConfiguration<UserUpvotes>
    {
        public void Configure(EntityTypeBuilder<UserUpvotes> builder)
        {
            builder.ToTable("USER_UPVOTES").HasKey(x => x.Id);

            builder.Property(x => x.UserToken).HasColumnName("USER_TOKEN").HasMaxLength(128);
            builder.Property(x => x.Json).HasColumnName("JSON");

            builder.HasIndex(x => x.UserToken);
        }
    }
}