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
    public class RefreshToken
    {
        private readonly CloudTable _usersTable;
        private readonly TokenDto _refreshTokenRequest;
        private readonly ILogger _logger;

        public RefreshToken(
            TokenDto refreshTokenRequest,
            CloudTable usersTable,
            ILogger logger)
        {
            _refreshTokenRequest = refreshTokenRequest;
            _usersTable = usersTable;
            _logger = logger;
        }

        public async Task<IActionResult> ProcessAsync()
        {
            await _usersTable.CreateIfNotExistsAsync();

            try
            {
                Validator.ValidateRequest(_refreshTokenRequest);

                var validToken = TokenManager.ValidateJSONWebToken(
                    _refreshTokenRequest.Token,
                    validateLifetime: false,
                    out _,
                    out var email,
                    out var refreshToken);

                if (validToken)
                {
                    if (refreshToken == _refreshTokenRequest.RefreshToken)
                    {
                        var existingUsers = _usersTable
                            .CreateQuery<UserDto>()
                            .AsQueryable()
                            .Where(u => u.NormalizedEmail == email)
                            .ToArray();

                        if (existingUsers.Any())
                        {
                            var user = existingUsers[0];

                            var token = TokenManager.GenerateJSONWebToken(user);
                            return new OkObjectResult(token);
                        }
                    }
                }

                _logger.LogWarning("Invalid token refresh attempt");
                return new BadRequestObjectResult(new { errors = "Invalid token" });

            }
            catch (ValidationException e)
            {
                return new BadRequestObjectResult(new { errors = e.Message });
            }
        }
    }
}
