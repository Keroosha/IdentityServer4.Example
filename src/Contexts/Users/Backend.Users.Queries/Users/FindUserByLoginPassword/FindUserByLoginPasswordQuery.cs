using System;
using System.Collections.Generic;
using System.Text;
using Backend.Infrastructure.CQRS.Queries;
using Backend.Users.Domain.Aggregates.UserAggregate;

namespace Backend.Users.Queries.Users.FindUserByLoginPassword
{
    public class FindUserByLoginPasswordQuery : IQuery<User>
    {
        public FindUserByLoginPasswordQuery(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public string Login { get; }

        public string Password { get; }
    }
}