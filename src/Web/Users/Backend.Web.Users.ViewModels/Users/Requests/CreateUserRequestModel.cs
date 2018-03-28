using System.ComponentModel.DataAnnotations;

namespace Backend.Web.Users.ViewModels.Users.Requests
{
    public class CreateUserRequestModel
    {
        [Required] [MaxLength(100)] public string Login { get; set; }

        [Required] [MaxLength(100)] public string Password { get; set; }
    }
}