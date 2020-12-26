namespace CCA.Application.Services.Security
{
    public class LoginToken
    {
        public LoginToken(
            string token,
            string refreshToken)
        {
            Token = token;
            RefreshToken = refreshToken;
        }

        public string Token { get; set; }


        public string RefreshToken { get; set; }
    }
}
