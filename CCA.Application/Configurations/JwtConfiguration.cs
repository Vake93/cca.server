using System;

namespace CCA.Application.Configurations
{
    public class JwtConfiguration
    {
        public string SecretKey => Environment.GetEnvironmentVariable("JwtSecretKey");

        public string Issuer => Environment.GetEnvironmentVariable("JwtIssuer");

        public string Audience => Environment.GetEnvironmentVariable("JwtAudience");

        public int Expires
        {
            get
            {
                var expireText = Environment.GetEnvironmentVariable("JwtExpires");
                if (!int.TryParse(expireText, out var expires))
                {
                    return 5;
                }

                return expires;
            }
        }
    }
}
