using AutoMapper;
using Backend.Users.Commands.Users.CreateUser;
using Backend.Users.Domain.Aggregates.UserAggregate;
using Backend.Users.Queries.Users.FindUserById;
using Backend.Users.Queries.Users.FindUserByLoginPassword;
using Backend.Web.Users.ViewModels.Users.Requests;
using Backend.Web.Users.ViewModels.Users.Responses;

namespace Backend.Web.Users.Configuration.Automapper.Users
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            // Requests
            CreateMap<CreateUserRequestModel, CreateUserCommand>();
            CreateMap<long, FindUserByIdQuery>()
                .ConvertUsing(src => new FindUserByIdQuery(src));
            CreateMap<FindUserByLoginPasswordRequestModel, FindUserByLoginPasswordQuery>();

            // Responses
            CreateMap<User, UserViewModel>();
        }
    }
}