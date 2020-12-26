using Newtonsoft.Json;

namespace CCA.Application.Services.Authentication.Microsoft
{
    public class MicrosoftUserResponse : IOpenIDUserProfile
    {
        public string Id { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        [JsonProperty("givenName")]
        public string FirstName { get; set; } = string.Empty;

        public string JobTitle { get; set; } = string.Empty;

        public string MobilePhone { get; set; } = string.Empty;

        [JsonProperty("surname")]
        public string LastName { get; set; } = string.Empty;

        [JsonProperty("userPrincipalName")]
        public string Email { get; set; } = string.Empty;
    }
}
