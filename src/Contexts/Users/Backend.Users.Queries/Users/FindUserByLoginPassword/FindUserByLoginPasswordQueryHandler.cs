using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Backend.Infrastructure.CQRS.Queries;
using Backend.Users.DataAccess;
using Backend.Users.Domain.Aggregates.UserAggregate;
using Backend.Users.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace Backend.Users.Queries.Users.FindUserByLoginPassword
{
    public class FindUserByLoginPasswordQueryHandler : IQueryHandler<FindUserByLoginPasswordQuery, User>
    {
        private readonly UsersDbContext _dbContext;
        private readonly IPasswordValidator _passwordValidator;

        public FindUserByLoginPasswordQueryHandler(
            UsersDbContext dbContext,
            IPasswordValidator passwordValidator)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _passwordValidator = passwordValidator ?? throw new ArgumentNullException(nameof(passwordValidator));
        }

        public async Task<User> Handle(
            FindUserByLoginPasswordQuery query,
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
                        .FirstOrDefaultAsync(
                            x => x.UserName == query.Login,
                            cancellationToken);
                    if (user != null && _passwordValidator.IsPasswordValid(user, query.Password))
                    {
                        return user;
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return null;
        }
    }
}