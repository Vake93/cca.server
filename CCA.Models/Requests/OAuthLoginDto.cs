using Newtonsoft.Json;

namespace CCA.Models.Requests
{
    public class OAuthLoginDto
    {
        public string Provider { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;


        [JsonIgnore]
        public AuthenticationProviderType ProviderType { get; set; }
    }
}
