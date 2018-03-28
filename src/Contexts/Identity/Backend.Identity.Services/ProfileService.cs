using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Backend.Common.Users.Client;
using Backend.Web.Users.ViewModels.Users.Responses;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace Backend.Identity.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UsersApiClient _apiClient;

        public ProfileService(UsersApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var userId = long.Parse(context.Subject.GetSubjectId());
            var user = await _apiClient.FindUserByIdAsync(userId);
            var userClaims = ConvertUserToClaims(user);
            context.AddRequestedClaims(userClaims);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var userId = long.Parse(context.Subject.GetSubjectId());
            var user = await _apiClient.FindUserByIdAsync(userId);
            context.IsActive = user != null;
        }

        private IEnumerable<Claim> ConvertUserToClaims(UserViewModel user)
        {
            if (user == null)
                return new Claim[] { };
            var result = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("name", user.UserName)
            };
            return result;
        }
    }
}