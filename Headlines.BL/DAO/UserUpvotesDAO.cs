using Headlines.ORM.Core.Entities;
using Microsoft.EntityFrameworkCore;
using PBilek.Infrastructure.DatetimeProvider;
using PBilek.ORM.Core.UnitOfWork;
using PBilek.ORM.EntityFrameworkCore.DAO;

namespace Headlines.BL.DAO
{
    public sealed class UserUpvotesDao : EfCoreDAO<UserUpvotes, long>, IUserUpvotesDao
    {
        public UserUpvotesDao(IUnitOfWorkProvider provider, IDateTimeProvider dateTimeProvider) : base(provider, dateTimeProvider)
        {
        }

        public Task<UserUpvotes> GetByUserTokenAsync(string token, CancellationToken cancellationToken)
        {
            return DbContext.Set<UserUpvotes>()
                .Where(x => x.UserToken == token)
                .FirstOrDefaultAsync(cancellationToken)!;
        }

        public Task<List<UserUpvotes>> GetAllAsync(CancellationToken cancellationToken)
        {
            return DbContext.Set<UserUpvotes>()
                .ToListAsync(cancellationToken);
        }
    }
}