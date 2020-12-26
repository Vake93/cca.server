namespace CCA.Application.Services.Security
{
    public class HashedPassword
    {
        public HashedPassword(string passwordHash, string passwordSalt)
        {
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
        }

        public string PasswordHash { get; }

        public string PasswordSalt { get; }
    }
}
