using System;
using System.Threading;
using System.Threading.Tasks;
using Backend.Infrastructure.CQRS.Commands;
using Backend.Users.DataAccess;
using Backend.Users.Domain.Aggregates.UserAggregate;

namespace Backend.Users.Commands.Users.CreateUser
{
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, long>
    {
        private readonly UsersDbContext _dbContext;
        private readonly UserFactory _userFactory;

        public CreateUserCommandHandler(UsersDbContext dbContext, UserFactory userFactory)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _userFactory = userFactory ?? throw new ArgumentNullException(nameof(userFactory));
        }

        public async Task<long> Handle(
            CreateUserCommand command,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    var user = _userFactory.Create(command.Login, command.Password);

                    _dbContext.Users.Add(user);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    transaction.Commit();
                    return user.Id;
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