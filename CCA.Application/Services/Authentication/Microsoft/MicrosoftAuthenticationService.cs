using CCA.Application.Configurations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace CCA.Application.Services.Authentication.Microsoft
{
    public class MicrosoftAuthenticationService : IOpenIDProvider
    {
        private const string _loginStatePrefix = "AAD-";
        private const string _loginUrl = "https://login.microsoftonline.com/common/oauth2/v2.0/authorize?response_type=code&response_mode=query&scope=openid%20email%20user.read";
        private const string _token = "https://login.microsoftonline.com/common/oauth2/v2.0/token";
        private const string _userInfo = "https://graph.microsoft.com/v1.0/me";

        private readonly MicrosoftAuthConfiguration _microsoftAuthConfiguration;
        private readonly HttpClient _httpClient;

        public static readonly MicrosoftAuthenticationService Instance = new MicrosoftAuthenticationService();

        private MicrosoftAuthenticationService()
        {
            _microsoftAuthConfiguration = new MicrosoftAuthConfiguration();
            _httpClient = new HttpClient();
        }

        public string GetLoginUrl()
        {
            var tenantId = HttpUtility.UrlEncode(_microsoftAuthConfiguration.TenantId);
            var clientId = HttpUtility.UrlEncode(_microsoftAuthConfiguration.ClientId);
            var state = HttpUtility.UrlEncode($"{_loginStatePrefix}{Guid.NewGuid()}");
            var redirectURI = HttpUtility.UrlEncode(_microsoftAuthConfiguration.RedirectURI);

            return
               _loginUrl +
               $"&tenant_id={tenantId}" +
               $"&client_id={clientId}" +
               $"&redirect_uri={redirectURI}" +
               $"&state={state}";
        }

        public async Task<IOpenIDUserProfile?> GetProfileAsync(string code)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, _userInfo);

            var token = await GetAzureActiveDirectoryTokenAsync(code);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            var content = await response.Content.ReadAsStringAsync();
            var azureActiveDirectoryUser = JsonConvert.DeserializeObject<MicrosoftUserResponse>(content);

            if (azureActiveDirectoryUser?.Email is null)
            {
                return null;
            }

            return azureActiveDirectoryUser;
        }

        private async Task<string> GetAzureActiveDirectoryTokenAsync(string code)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, _token)
            {
                Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("client_id", _microsoftAuthConfiguration.ClientId),
                    new KeyValuePair<string, string>("client_secret", _microsoftAuthConfiguration.ClientSecret),
                    new KeyValuePair<string, string>("redirect_uri", _microsoftAuthConfiguration.RedirectURI),
                    new KeyValuePair<string, string>("grant_type", "authorization_code")
                })
            };

            request
                .Headers
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            var response = await _httpClient.SendAsync(request);
            var bodyContent = await response.Content.ReadAsStringAsync();

            var openIdToken = JsonConvert.DeserializeObject<OpenIDToken>(bodyContent);

            return openIdToken.AccessToken;
        }
    }
}
