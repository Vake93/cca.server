using CCA.Application.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ProfileHandler = CCA.Application.Handlers.Users.Profile;

namespace CCA.User.Service
{
    public static class Profile
    {
        [FunctionName("profile")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, HttpMethod.GET, Route = "users/profile")] HttpRequest request,
            [Table("Users", Connection = "ConnectionStrings:StorageConnectionString")] CloudTable usersTable,
            ILogger logger)
        {
            if (request.AuthenticateRequest(out var _, out var loginUserEmail))
            {
                return await new ProfileHandler(loginUserEmail, usersTable, logger).ProcessAsync();
            }

            logger.LogWarning("Unauthorized Request to join-meeting");
            return new UnauthorizedResult();
        }
    }
}
