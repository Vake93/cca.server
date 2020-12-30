namespace CCA.Models.Requests
{
    public class OAuthLoginDto
    {
        public string State { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;
    }
}
