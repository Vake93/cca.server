using CCA.Application.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using HealthCheckHandler = CCA.Application.Handlers.HealthChecks.HealthCheck;

namespace CCA.Health.Service
{
    public static class HealthCheck
    {
        [FunctionName("health-check")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, HttpMethod.GET, Route = "health-check")] HttpRequest request,
            ILogger logger)
        {
            var remoteAddress = $"{request.HttpContext.Connection.RemoteIpAddress}";
            return await new HealthCheckHandler(remoteAddress, logger).ProcessAsync();
        }
    }
}
