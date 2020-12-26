using CCA.Application.Services.Security;
using CCA.Models.Requests;
using CCA.Models.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using UserDto = CCA.Models.Storage.User;

namespace CCA.Application.Handlers.Users
{
    public class Login
    {
        private readonly CloudTable _usersTable;
        private readonly LoginDto _loginRequest;
        private readonly ILogger _logger;

        public Login(
            LoginDto loginRequest,
            CloudTable usersTable,
            ILogger logger)
        {
            _usersTable = usersTable;
            _loginRequest = loginRequest;
            _logger = logger;
        }

        public async Task<IActionResult> ProcessAsync()
        {
            await _usersTable.CreateIfNotExistsAsync();

            try
            {
                Validator.ValidateRequest(_loginRequest);

                var normalizedEmail = _loginRequest.Email.ToUpperInvariant();

                var existingUsers = _usersTable
                    .CreateQuery<UserDto>()
                    .AsQueryable()
                    .Where(u => u.NormalizedEmail == normalizedEmail)
                    .ToArray();

                if (existingUsers.Any())
                {
                    var user = existingUsers[0];

                    if (!string.IsNullOrEmpty(user.PasswordHash) && !string.IsNullOrEmpty(user.PasswordSalt))
                    {
                        var hashPassword = new HashedPassword(user.PasswordHash, user.PasswordSalt);

                        if (CredentialManager.ValidatePassword(_loginRequest.Password, hashPassword))
                        {
                            var token = TokenManager.GenerateJSONWebToken(user);
                            return new OkObjectResult(token);
                        }

                        _logger.LogWarning("Invalid password for user: {0}", user.Email);
                    }
                }

                return new UnauthorizedObjectResult(new { errors = "Invalid email and/or password" });

            }
            catch (ValidationException e)
            {
                return new BadRequestObjectResult(new { errors = e.Message });
            }
        }
    }
}
