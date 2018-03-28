using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Backend.Web.Users.ViewModels.Users.Requests
{
    public class FindUserByLoginPasswordRequestModel
    {
        [Required] [MaxLength(100)]
        public string Login { get; set; }

        [Required] [MaxLength(100)]
        public string Password { get; set; }
    }
}