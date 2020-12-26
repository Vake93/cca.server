using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CCA.Application.Services.Security
{
    public static class CredentialManager
    {
        private static readonly RNGCryptoServiceProvider _cryptoServiceProvider = new RNGCryptoServiceProvider();
        private static readonly HashAlgorithm _hashAlgorithm = new SHA256Managed();

        public static HashedPassword HashPassword(string password)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);

            var saltBytes = new byte[32];
            _cryptoServiceProvider.GetBytes(saltBytes);

            var passwordWithSalt = new byte[passwordBytes.Length + saltBytes.Length];

            Array.Copy(saltBytes, 0, passwordWithSalt, 0, saltBytes.Length);
            Array.Copy(passwordBytes, 0, passwordWithSalt, saltBytes.Length, passwordBytes.Length);

            var passwordHash = _hashAlgorithm.ComputeHash(passwordWithSalt);

            return new HashedPassword(
                Convert.ToBase64String(passwordHash),
                Convert.ToBase64String(saltBytes));
        }

        public static bool ValidatePassword(string password, HashedPassword hashedPassword)
        {
            var passwordText = !string.IsNullOrEmpty(password) ? password : throw new ArgumentNullException(nameof(password));
            var saltText = !string.IsNullOrEmpty(hashedPassword?.PasswordSalt) ? hashedPassword.PasswordSalt : throw new ArgumentNullException(nameof(hashedPassword));
            var checkHashedPasswordText = !string.IsNullOrEmpty(hashedPassword?.PasswordHash) ? hashedPassword.PasswordHash : throw new ArgumentNullException(nameof(hashedPassword));

            var passwordBytes = Encoding.UTF8.GetBytes(passwordText);
            var saltBytes = Convert.FromBase64String(saltText);
            var checkHashedPassword = Convert.FromBase64String(checkHashedPasswordText);

            var passwordWithSalt = new byte[passwordBytes.Length + saltBytes.Length];

            Array.Copy(saltBytes, 0, passwordWithSalt, 0, saltBytes.Length);
            Array.Copy(passwordBytes, 0, passwordWithSalt, saltBytes.Length, passwordBytes.Length);

            var passwordHash = _hashAlgorithm.ComputeHash(passwordWithSalt);

            return checkHashedPassword.Length == passwordHash.Length &&
                   checkHashedPassword.SequenceEqual(passwordHash);
        }
    }
}
