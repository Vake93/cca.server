using CCA.Application.Extensions;
using CCA.Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using JoinMeetingHandler = CCA.Application.Handlers.DialIn.JoinMeeting;

namespace CCA.DialIn.Service
{
    public static class JoinMeeting
    {
        [FunctionName("dialin-meeting")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, HttpMethod.POST, Route = "dialin/meeting")] HttpRequest request,
            [Table("Events", Connection = "ConnectionStrings:StorageConnectionString")] CloudTable eventsTable,
            ILogger logger)
        {
            if (request.HasFormContentType)
            {
                var joinMeetingRequest = new DialInJoinMeeting
                {
                    AccountSid = request.Form[nameof(DialInJoinMeeting.AccountSid)],
                    Caller = request.Form[nameof(DialInJoinMeeting.Caller)],
                    Digits = request.Form[nameof(DialInJoinMeeting.Digits)],
                };

                return await new JoinMeetingHandler(joinMeetingRequest, eventsTable, logger).ProcessAsync();
            }
            else
            {
                return new UnsupportedMediaTypeResult();
            }
        }
    }
}
