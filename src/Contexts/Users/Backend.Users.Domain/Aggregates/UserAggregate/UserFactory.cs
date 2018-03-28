using System;
using Backend.Users.Domain.Services;

namespace Backend.Users.Domain.Aggregates.UserAggregate
{
    public class UserFactory
    {
        private readonly IPasswordHasher _passwordHasher;

        public UserFactory(IPasswordHasher passwordHasher)
        {
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        public User Create(string userName, string password)
        {
            var passwordHash = _passwordHasher.ComputeHash(password);
            var newUser = new User(userName, passwordHash);
            return newUser;
        }
    }
}