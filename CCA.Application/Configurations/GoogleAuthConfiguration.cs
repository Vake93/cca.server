using System;

namespace CCA.Application.Configurations
{
    public class GoogleAuthConfiguration
    {
        public string ClientId => Environment.GetEnvironmentVariable("GoogleAuthClientId");

        public string ClientSecret => Environment.GetEnvironmentVariable("GoogleAuthClientSecret");

        public string RedirectURI => Environment.GetEnvironmentVariable("GoogleAuthUri");
    }
}
