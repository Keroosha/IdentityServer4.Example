using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Backend.Web.Users.ViewModels.Users.Requests;
using Backend.Web.Users.ViewModels.Users.Responses;
using Newtonsoft.Json;

namespace Backend.Common.Users.Client
{
    public class UsersApiClient
    {
        private readonly Uri _baseUri;

        public UsersApiClient(UsersApiClientOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            _baseUri = new Uri(options.Location);
        }

        private Uri BuildUri(string relativeLocation)
        {
            return new Uri(_baseUri, new Uri(relativeLocation, UriKind.Relative));
        }

        public async Task<UserViewModel> CreateAsync(
            CreateUserRequestModel createModel,
            CancellationToken cancellationToken = default)
        {
            if (createModel == null)
                throw new ArgumentNullException(nameof(createModel));
            using (var client = new HttpClient())
            {
                var requestUri = BuildUri("api/users");
                var request = new StringContent(
                    JsonConvert.SerializeObject(createModel),
                    Encoding.UTF8,
                    "application/json");
                var httpResponse = await client.PostAsync(requestUri, request, cancellationToken);
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    var responseJson = await httpResponse.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<UserViewModel>(responseJson);
                    return user;
                }

                return null;
            }
        }

        public async Task<UserViewModel> FindUserByIdAsync(
            long userId,
            CancellationToken cancellationToken = default)
        {
            using (var client = new HttpClient())
            {
                var requestUri = BuildUri($"api/users/{userId:D}");
                var httpResponse = await client.GetAsync(requestUri, cancellationToken);
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    var responseJson = await httpResponse.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<UserViewModel>(responseJson);
                    return user;
                }

                return null;
            }
        }

        public async Task<UserViewModel> FindUserByLoginPasswordAsync(
            string login,
            string password,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(login))
                return null;
            if (string.IsNullOrEmpty(password))
                return null;
            using (var client = new HttpClient())
            {
                var uriBuilder = new UriBuilder(BuildUri("api/users"))
                {
                    Query = $"login={login}&password={password}"
                };
                var requestUri = uriBuilder.Uri;

                var httpResponse = await client.GetAsync(requestUri, cancellationToken);
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    var responseJson = await httpResponse.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<UserViewModel>(responseJson);
                    return user;
                }

                return null;
            }
        }
    }
}