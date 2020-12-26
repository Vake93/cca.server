using CCA.Application.Extensions;
using CCA.Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WelcomeHandler = CCA.Application.Handlers.DialIn.Welcome;

namespace CCA.DialIn.Service
{
    public static class Welcome
    {
        [FunctionName("dialin")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, HttpMethod.POST, Route = "dialin")] HttpRequest request,
            ILogger logger)
        {

            if (request.HasFormContentType)
            {
                var joinMeetingRequest = new DialInWelcome
                {
                    AccountSid = request.Form[nameof(DialInWelcome.AccountSid)],
                    Caller = request.Form[nameof(DialInWelcome.Caller)]
                };

                return await new WelcomeHandler(joinMeetingRequest, logger).ProcessAsync();
            }
            else
            {
                return new UnsupportedMediaTypeResult();
            }
        }
    }
}
