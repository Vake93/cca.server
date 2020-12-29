using CCA.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using UserDto = CCA.Models.Storage.User;

namespace CCA.Application.Handlers.Users
{
    public class Profile
    {
        private readonly CloudTable _usersTable;
        private readonly string _loginUserEmail;
        private readonly ILogger _logger;

        public Profile(
            string loginUserEmail,
            CloudTable usersTable,
            ILogger logger)
        {
            _loginUserEmail = loginUserEmail;
            _usersTable = usersTable;
            _logger = logger;
        }

        public async Task<IActionResult> ProcessAsync()
        {
            await _usersTable.CreateIfNotExistsAsync();

            var existingUsers = _usersTable
                    .CreateQuery<UserDto>()
                    .AsQueryable()
                    .Where(u => u.NormalizedEmail == _loginUserEmail)
                    .ToArray();

            if (existingUsers.Any())
            {
                var user = existingUsers[0];

                return new OkObjectResult(new ProfileDto
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                });
            }

            _logger.LogError("User {0} not found", _loginUserEmail);
            return new UnauthorizedObjectResult(new { errors = "User not found." });
        }
    }
}
