using Backend.Infrastructure.CQRS.Commands;

namespace Backend.Users.Commands.Users.CreateUser
{
    public class CreateUserCommand : ICommand<long>
    {
        public CreateUserCommand(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public string Login { get; }

        public string Password { get; }
    }
}