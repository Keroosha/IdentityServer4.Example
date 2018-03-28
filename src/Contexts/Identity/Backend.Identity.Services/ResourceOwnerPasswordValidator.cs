using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Backend.Common.Users.Client;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace Backend.Identity.Services
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UsersApiClient _apiClient;

        public ResourceOwnerPasswordValidator(UsersApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = await _apiClient.FindUserByLoginPasswordAsync(
                context.UserName,
                context.Password);
            if (user != null)
            {
                context.Result = new GrantValidationResult(
                    user.Id.ToString(CultureInfo.InvariantCulture),
                    OidcConstants.AuthenticationMethods.Password);
            }
            else
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
            }
        }
    }
}