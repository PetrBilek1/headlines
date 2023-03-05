using Microsoft.EntityFrameworkCore;
using PBilek.ORM.EntityFrameworkCore.SQL.Context;

namespace Headlines.ORM.Core.Context
{
    public sealed class HeadlinesDbContext : EfCoreSqlDbContext
    {
        public HeadlinesDbContext(DbContextOptions<HeadlinesDbContext> options) : base(options)
        {
        }    
    }
}