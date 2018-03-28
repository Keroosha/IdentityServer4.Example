using System;
using System.Collections.Generic;
using System.Text;
using Backend.Infrastructure.CQRS.Queries;
using Backend.Users.Domain.Aggregates.UserAggregate;

namespace Backend.Users.Queries.Users.FindUserById
{
    public class FindUserByIdQuery : IQuery<User>
    {
        public FindUserByIdQuery(long userId)
        {
            UserId = userId;
        }

        public long UserId { get; }
    }
}