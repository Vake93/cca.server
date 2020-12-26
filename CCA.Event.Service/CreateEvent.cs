using CCA.Application.Extensions;
using CCA.Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using CreateEventHandler = CCA.Application.Handlers.Events.CreateEvent;
using ListEventsHandler = CCA.Application.Handlers.Events.ListEvents;

namespace CCA.Event.Service
{
    public static class CreateEvent
    {
        [FunctionName("create-event")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, HttpMethod.GET, HttpMethod.POST, Route = "events")] HttpRequest request,
            [Table("Events", Connection = "ConnectionStrings:StorageConnectionString")] CloudTable eventsTable,
            ILogger logger)
        {
            await eventsTable.CreateIfNotExistsAsync();

            if (request.AuthenticateRequest(out var loginUserId, out var loginUserEmail))
            {
                if (request.Method == HttpMethod.POST)
                {
                    var newEventRequest = await request.DeserializeRequestAsync<NewEventDto>();
                    return await new CreateEventHandler(newEventRequest, eventsTable, loginUserEmail, loginUserId, logger).ProcessAsync();
                }

                if (request.Method == HttpMethod.GET)
                {
                    var listEventsRequest = new ListEventsDto
                    {
                        Skip = request.QueryParam("skip", 0),
                        Limit = request.QueryParam("limit", 10),
                        Date = request.QueryParam("date", (DateTime?)null)
                    };

                    return await new ListEventsHandler(listEventsRequest, eventsTable, loginUserEmail).ProcessAsync();
                }
            }

            logger.LogWarning("Unauthorized Request to create-event");
            return new UnauthorizedResult();
        }
    }
}
