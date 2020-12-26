using Newtonsoft.Json;

namespace CCA.Models.Requests
{
    public class OAuthUrlDto
    {
        public string Provider { get; set; } = string.Empty;

        [JsonIgnore]
        public AuthenticationProviderType ProviderType { get; set; }
    }
}
