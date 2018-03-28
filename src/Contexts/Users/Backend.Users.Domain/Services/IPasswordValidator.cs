using System;
using System.Collections.Generic;
using System.Text;
using Backend.Users.Domain.Aggregates.UserAggregate;

namespace Backend.Users.Domain.Services
{
    public interface IPasswordValidator
    {
        bool IsPasswordValid(User user, string providedPassword);
    }
}