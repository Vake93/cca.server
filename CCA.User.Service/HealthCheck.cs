using CCA.Application.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CCA.User.Service
{
    public static class HealthCheck
    {
        [FunctionName("health-check")]
        public static Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, HttpMethod.GET, Route = "users/health-check")] HttpRequest request,
            ILogger log)
        {
            var remoteAddress = request.HttpContext.Connection.RemoteIpAddress;
            log.LogInformation($"Running health-check API for {remoteAddress}");
            return Task.FromResult((IActionResult)new OkObjectResult(new { status = "Healthy" }));
        }
    }
}
