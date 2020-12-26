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
    public class Registration
    {
        private readonly RegisterDto _registerRequest;
        private readonly CloudTable _usersTable;
        private readonly ILogger _logger;

        public Registration(
            RegisterDto registerRequest,
            CloudTable usersTable,
            ILogger logger)
        {
            _registerRequest = registerRequest;
            _usersTable = usersTable;
            _logger = logger;
        }

        public async Task<IActionResult> ProcessAsync()
        {
            await _usersTable.CreateIfNotExistsAsync();

            try
            {
                Validator.ValidateRequest(_registerRequest);

                var existingUsers = _usersTable
                    .CreateQuery<UserDto>()
                    .AsQueryable()
                    .Where(u => u.Email == _registerRequest.Email)
                    .ToArray();

                if (existingUsers.Any())
                {
                    return new ConflictObjectResult(new { errors = "User with the same email already exists." });
                }

                var hashPassword = CredentialManager.HashPassword(_registerRequest.Password);

                var user = new UserDto
                {
                    Email = _registerRequest.Email,
                    NormalizedEmail = _registerRequest.Email.ToUpperInvariant(),
                    FirstName = _registerRequest.FirstName,
                    LastName = _registerRequest.LastName,
                    PasswordHash = hashPassword.PasswordHash,
                    PasswordSalt = hashPassword.PasswordSalt,
                    Timestamp = DateTime.UtcNow,
                };

                await _usersTable.ExecuteAsync(TableOperation.Insert(user));

                _logger.LogInformation("Created new user with id: {0}", user.RowKey);

                return new CreatedResult(string.Empty, new { id = user.RowKey });
            }
            catch (ValidationException e)
            {
                return new BadRequestObjectResult(new { errors = e.Message });
            }
        }
    }
}
