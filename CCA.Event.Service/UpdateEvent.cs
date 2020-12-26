using CCA.Application.Extensions;
using CCA.Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using DeleteEventHandler = CCA.Application.Handlers.Events.DeleteEvent;
using UpdateEventHandler = CCA.Application.Handlers.Events.UpdateEvent;

namespace CCA.Event.Service
{
    public static class UpdateEvent
    {
        [FunctionName("update-event")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, HttpMethod.PUT, HttpMethod.DELETE, Route = "events/{id}")] HttpRequest request,
            [Table("Events", Connection = "ConnectionStrings:StorageConnectionString")] CloudTable eventsTable,
            string id,
            ILogger logger)
        {
            await eventsTable.CreateIfNotExistsAsync();

            if (request.AuthenticateRequest(out var loginUserId, out var loginUserEmail))
            {
                if (request.Method == HttpMethod.PUT)
                {
                    var updateEventRequest = await request.DeserializeRequestAsync<UpdateEventDto>();
                    updateEventRequest.Id = id;

                    return await new UpdateEventHandler(updateEventRequest, eventsTable, loginUserEmail, loginUserId, logger).ProcessAsync();
                }

                if (request.Method == HttpMethod.DELETE)
                {
                    var deleteEventRequest = new DeleteEventDto
                    {
                        Id = id
                    };

                    return await new DeleteEventHandler(deleteEventRequest, eventsTable, loginUserId, logger).ProcessAsync();
                }
            }

            logger.LogWarning("Unauthorized Request to update-event");
            return new UnauthorizedResult();
        }
    }
}
