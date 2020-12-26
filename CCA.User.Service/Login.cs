using CCA.Application.Extensions;
using CCA.Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using LoginHandler = CCA.Application.Handlers.Users.Login;

namespace CCA.User.Service
{
    public static class Login
    {
        [FunctionName("login")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, HttpMethod.POST, Route = "users/login")] HttpRequest request,
            [Table("Users", Connection = "ConnectionStrings:StorageConnectionString")] CloudTable usersTable,
            ILogger logger)
        {
            var loginRequest = await request.DeserializeRequestAsync<LoginDto>();
            return await new LoginHandler(loginRequest, usersTable, logger).ProcessAsync();
        }
    }
}
