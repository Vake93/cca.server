using CCA.Application.Services.Authentication;
using CCA.Application.Services.Authentication.Google;
using CCA.Application.Services.Authentication.Microsoft;
using CCA.Application.Services.Security;
using CCA.Models.Requests;
using CCA.Models.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using UserDto = CCA.Models.Storage.User;

namespace CCA.Application.Handlers.Users
{
    public class OAuthLogin
    {
        private readonly CloudTable _usersTable;
        private readonly OAuthLoginDto _loginRequest;
        private readonly ILogger _logger;

        public OAuthLogin(
            OAuthLoginDto loginRequest,
            CloudTable usersTable,
            ILogger logger)
        {
            _loginRequest = loginRequest;
            _usersTable = usersTable;
            _logger = logger;
        }

        public async Task<IActionResult> ProcessAsync()
        {
            await _usersTable.CreateIfNotExistsAsync();

            try
            {
                Validator.ValidateRequest(_loginRequest);

                var openIDProvider = _loginRequest.ProviderType == AuthenticationProviderType.Microsoft ? (IOpenIDProvider)
                    MicrosoftAuthenticationService.Instance :
                    GoogleAuthenticationService.Instance;

                var openIDUserProfile = await openIDProvider.GetProfileAsync(_loginRequest.Token);

                if (openIDUserProfile is { })
                {
                    var normalizedEmail = openIDUserProfile.Email.ToUpperInvariant();

                    var existingUsers = _usersTable
                        .CreateQuery<UserDto>()
                        .AsQueryable()
                        .Where(u => u.NormalizedEmail == normalizedEmail)
                        .ToArray();

                    if (existingUsers.Any())
                    {
                        var user = existingUsers[0];
                        var token = TokenManager.GenerateJSONWebToken(user);
                        return new OkObjectResult(token);
                    }
                    else
                    {
                        var user = new UserDto
                        {
                            Email = openIDUserProfile.Email,
                            NormalizedEmail = normalizedEmail,
                            FirstName = openIDUserProfile.FirstName,
                            LastName = openIDUserProfile.LastName,
                            PasswordHash = string.Empty,
                            PasswordSalt = string.Empty,
                            Timestamp = DateTime.UtcNow,
                        };

                        await _usersTable.ExecuteAsync(TableOperation.Insert(user));

                        _logger.LogInformation("Created new user with id: {0}", user.RowKey);

                        var token = TokenManager.GenerateJSONWebToken(user);
                        return new OkObjectResult(token);
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
