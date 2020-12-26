using CCA.Application.Extensions;
using CCA.Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using JoinMeetingHandler = CCA.Application.Handlers.Meetings.JoinMeeting;

namespace CCA.Twilio.Service
{
    public static class JoinMeeting
    {
        [FunctionName("join-meeting")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, HttpMethod.POST, Route = "meeting/{eventId}")] HttpRequest request,
            [Table("Events", Connection = "ConnectionStrings:StorageConnectionString")] CloudTable eventsTable,
            string eventId,
            ILogger logger)
        {
            await eventsTable.CreateIfNotExistsAsync();

            if (request.AuthenticateRequest(out var _, out var loginUserEmail))
            {
                var joinMeetingRequest = await request.DeserializeRequestAsync<JoinMeetingDto>();
                joinMeetingRequest.EventId = eventId;

                return await new JoinMeetingHandler(joinMeetingRequest, eventsTable, loginUserEmail, logger).ProcessAsync();
            }

            logger.LogWarning("Unauthorized Request to join-meeting");
            return new UnauthorizedResult();
        }
    }
}
