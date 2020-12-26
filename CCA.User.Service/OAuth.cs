using CCA.Application.Extensions;
using CCA.Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using OAuthLoginHandler = CCA.Application.Handlers.Users.OAuthLogin;
using OAuthUrlHandler = CCA.Application.Handlers.Users.OAuthUrl;

namespace CCA.User.Service
{
    public static class OAuth
    {
        [FunctionName("oauth")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, HttpMethod.GET, HttpMethod.POST, Route = "users/oauth")] HttpRequest request,
            [Table("Users", Connection = "ConnectionStrings:StorageConnectionString")] CloudTable usersTable,
            ILogger logger)
        {
            if (request.Method == HttpMethod.GET)
            {
                var oauthUrlRequest = new OAuthUrlDto
                {
                    Provider = request.Query["provider"]
                };

                return await new OAuthUrlHandler(oauthUrlRequest, logger).ProcessAsync();
            }

            if (request.Method == HttpMethod.POST)
            {
                var loginRequest = await request.DeserializeRequestAsync<OAuthLoginDto>();
                return await new OAuthLoginHandler(loginRequest, usersTable, logger).ProcessAsync();
            }

            return new BadRequestResult();
        }
    }
}
