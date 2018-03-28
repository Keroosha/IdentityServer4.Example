using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Web.Users.ViewModels.Users.Responses
{
    public class UserViewModel
    {
        public UserViewModel(long id, string userName)
        {
            Id = id;
            UserName = userName;
        }

        public long Id { get; }

        public string UserName { get; }
    }
}