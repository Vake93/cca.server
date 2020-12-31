using CCA.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Linq;
using System.Threading.Tasks;
using EventDto = CCA.Models.Storage.Event;

namespace CCA.Application.Handlers.Events
{
    public class ListEvents
    {
        private readonly ListEventsDto _listEventsRequest;
        private readonly CloudTable _eventsTable;
        private readonly string _loginUserEmail;

        public ListEvents(
            ListEventsDto listEventsRequest,
            CloudTable eventsTable,
            string loginUserEmail)
        {
            _listEventsRequest = listEventsRequest;
            _loginUserEmail = loginUserEmail;
            _eventsTable = eventsTable;
        }

        public async Task<IActionResult> ProcessAsync()
        {
            await _eventsTable.CreateIfNotExistsAsync();

            var allEvents = _eventsTable
                .CreateQuery<EventDto>()
                .AsQueryable()
                .ToArray();

            var events = Array.Empty<EventDto>();
            var count = 0;

            if (_listEventsRequest.Date.HasValue)
            {
                var start = _listEventsRequest.Date.Value.Date;
                var end = _listEventsRequest.Date.Value.Date.AddDays(1);

                var filterEvents = allEvents
                    .Where(e => e.EventGuests.Contains(_loginUserEmail) && e.StartTime >= start && e.EndTime < end);

                count = filterEvents.Count();
                events = filterEvents
                    .Skip(_listEventsRequest.Skip)
                    .Take(_listEventsRequest.Limit)
                    .OrderBy(e => e.StartTime)
                    .ToArray();
            }
            else
            {
                var filterEvents = allEvents
                    .Where(e => e.EventGuests.Contains(_loginUserEmail));

                count = filterEvents.Count();
                events = filterEvents
                    .Skip(_listEventsRequest.Skip)
                    .Take(_listEventsRequest.Limit)
                    .OrderBy(e => e.StartTime)
                    .ToArray();
            }

            return new OkObjectResult(new { items = events, count });
        }
    }
}
