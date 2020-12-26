using System;

namespace CCA.Application.Configurations
{
    public class MicrosoftAuthConfiguration
    {
        public string ClientId => Environment.GetEnvironmentVariable("MicrosoftAuthClientId");

        public string TenantId => Environment.GetEnvironmentVariable("MicrosoftAuthTenantId");

        public string ClientSecret => Environment.GetEnvironmentVariable("MicrosoftAuthClientSecret");

        public string RedirectURI => Environment.GetEnvironmentVariable("MicrosoftAuthUri");
    }
}
