using Newtonsoft.Json;

namespace CCA.Application.Services.Authentication.Google
{
    public class GoogleUserResponse : IOpenIDUserProfile
    {
        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;

        [JsonProperty("given_name")]
        public string FirstName { get; set; } = string.Empty;

        [JsonProperty("family_name")]
        public string LastName { get; set; } = string.Empty;

        [JsonProperty("exp")]
        public string Expire { get; set; } = string.Empty;

        [JsonProperty("picture")]
        public string Picture { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string FullName { get; set; } = string.Empty;

        [JsonProperty("email_verified")]
        public bool EmailVerified { get; set; }
    }
}
