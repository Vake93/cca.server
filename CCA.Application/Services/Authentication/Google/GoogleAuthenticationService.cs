using CCA.Application.Configurations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace CCA.Application.Services.Authentication.Google
{
    public class GoogleAuthenticationService : IOpenIDProvider
    {
        private const string _loginStatePrefix = "GCP-";
        private const string _loginUrl = "https://accounts.google.com/o/oauth2/v2/auth?response_type=code&scope=openid%20email";
        private const string _token = "https://oauth2.googleapis.com/token";
        private const string _userInfo = "https://www.googleapis.com/oauth2/v3/tokeninfo";

        private readonly GoogleAuthConfiguration _googleAuthConfiguration;
        private readonly HttpClient _httpClient;

        public static readonly GoogleAuthenticationService Instance = new GoogleAuthenticationService();

        private GoogleAuthenticationService()
        {
            _googleAuthConfiguration = new GoogleAuthConfiguration();
            _httpClient = new HttpClient();
        }

        public string GetLoginUrl()
        {
            var clientId = HttpUtility.UrlEncode(_googleAuthConfiguration.ClientId);
            var state = HttpUtility.UrlEncode($"{_loginStatePrefix}{Guid.NewGuid()}");
            var redirectURI = HttpUtility.UrlEncode(_googleAuthConfiguration.RedirectURI);

            return
               _loginUrl +
               $"&client_id={clientId}" +
               $"&redirect_uri={redirectURI}" +
               $"&state={state}";
        }

        public bool ValidState(string state)
        {
            return state.StartsWith(_loginStatePrefix);
        }

        public async Task<IOpenIDUserProfile?> GetProfileAsync(string code)
        {
            var token = await GetGoogleTokenAsync(code);

            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"{_userInfo}?id_token={token}");

            request
                .Headers
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var googleUser = JsonConvert.DeserializeObject<GoogleUserResponse>(content);

            if (!googleUser.EmailVerified)
            {
                return null;
            }

            return googleUser;
        }

        private async Task<string?> GetGoogleTokenAsync(string code)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, _token)
            {
                Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("client_id", _googleAuthConfiguration.ClientId),
                    new KeyValuePair<string, string>("client_secret", _googleAuthConfiguration.ClientSecret),
                    new KeyValuePair<string, string>("redirect_uri", _googleAuthConfiguration.RedirectURI),
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                })
            };

            request
                .Headers
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var bodyContent = await response.Content.ReadAsStringAsync();
                var openIdToken = JsonConvert.DeserializeObject<OpenIDToken>(bodyContent);
                return openIdToken.IdToken;
            }

            return null;
        }
    }
}
