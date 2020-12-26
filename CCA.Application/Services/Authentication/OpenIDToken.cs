using Newtonsoft.Json;

namespace CCA.Application.Services.Authentication
{
    public class OpenIDToken
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "id_token")]
        public string IdToken { get; set; } = string.Empty;
    }
}
