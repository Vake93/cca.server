using CCA.Application.Extensions;
using CCA.Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using RefreshTokenHandler = CCA.Application.Handlers.Users.RefreshToken;

namespace CCA.User.Service
{
    public static class RefreshToken
    {
        [FunctionName("refresh-token")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, HttpMethod.POST, Route = "users/token")] HttpRequest request,
            [Table("Users", Connection = "ConnectionStrings:StorageConnectionString")] CloudTable usersTable,
            ILogger logger)
        {
            var tokenRequest = await request.DeserializeRequestAsync<TokenDto>();
            return await new RefreshTokenHandler(tokenRequest, usersTable, logger).ProcessAsync();
        }
    }
}
