using CCA.Application.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace CCA.User.Service
{
    public static class HelloWorld
    {
        [FunctionName("hello-world")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, HttpMethod.GET, Route = "hello-world")] HttpRequest request)
        {
            if (request.Query.TryGetValue("name", out var name))
            {
                return new OkObjectResult($"Hello, {name}");
            }

            return new OkObjectResult("Hello, World");
        }
    }
}
