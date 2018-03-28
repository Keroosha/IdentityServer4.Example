using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Backend.Infrastructure.CQRS.Queries;
using Backend.Users.DataAccess;
using Backend.Users.Domain.Aggregates.UserAggregate;
using Microsoft.EntityFrameworkCore;

namespace Backend.Users.Queries.Users.FindUserById
{
    public class FindUserByIdQueryHandler : IQueryHandler<FindUserByIdQuery, User>
    {
        private readonly UsersDbContext _dbContext;

        public FindUserByIdQueryHandler(UsersDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<User> Handle(
            FindUserByIdQuery query,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var transaction = await _dbContext
                .Database
                .BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken))
            {
                try
                {
                    var user = await _dbContext
                        .Users
                        .SingleOrDefaultAsync(x => x.Id == query.UserId, cancellationToken);
                    return user;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}