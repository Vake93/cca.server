using CCA.Application.Extensions;
using CCA.Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using RegistrationHandler = CCA.Application.Handlers.Users.Registration;

namespace CCA.User.Service
{
    public static class Registration
    {
        [FunctionName("register")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, HttpMethod.POST, Route = "users/register")] HttpRequest request,
            [Table("Users", Connection = "ConnectionStrings:StorageConnectionString")] CloudTable usersTable,
            ILogger logger)
        {
            var registerRequest = await request.DeserializeRequestAsync<RegisterDto>();
            return await new RegistrationHandler(registerRequest, usersTable, logger).ProcessAsync();
        }
    }
}
