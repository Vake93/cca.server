using CCA.Models.Requests;
using CCA.Models.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using EventDto = CCA.Models.Storage.Event;

namespace CCA.Application.Handlers.Events
{
    public class UpdateEvent
    {
        private readonly UpdateEventDto _updateEventRequest;
        private readonly CloudTable _eventsTable;
        private readonly string _loginUserEmail;
        private readonly string _loginUserId;
        private readonly ILogger _logger;

        public UpdateEvent(
            UpdateEventDto updateEventRequest,
            CloudTable eventsTable,
            string loginUserEmail,
            string loginUserId,
            ILogger logger)
        {
            _updateEventRequest = updateEventRequest;
            _loginUserEmail = loginUserEmail;
            _loginUserId = loginUserId;
            _eventsTable = eventsTable;
            _logger = logger;
        }

        public async Task<IActionResult> ProcessAsync()
        {
            await _eventsTable.CreateIfNotExistsAsync();

            try
            {
                Validator.ValidateRequest(_updateEventRequest);

                var events = _eventsTable
                    .CreateQuery<EventDto>()
                    .AsQueryable()
                    .Where(e => e.RowKey == _updateEventRequest.Id)
                    .ToArray();

                if (events.Length == 0 || events[0].UserId != _loginUserId)
                {
                    return new NotFoundResult();
                }

                var @event = events[0];

                _updateEventRequest.EventGuests = _updateEventRequest.EventGuests
                    .Append(_loginUserEmail)
                    .Select(eg => eg.ToUpperInvariant())
                    .Distinct()
                    .ToArray();

                @event.Title = _updateEventRequest.Title;
                @event.StartTime = _updateEventRequest.StartTime;
                @event.EndTime = _updateEventRequest.EndTime;
                @event.Notes = _updateEventRequest.Notes;
                @event.Location = _updateEventRequest.Location;
                @event.Timestamp = DateTime.UtcNow;

                await _eventsTable.ExecuteAsync(TableOperation.Replace(@event));

                _logger.LogInformation("Updated event with id: {0}", @event.RowKey);

                return new OkResult();
            }
            catch (ValidationException e)
            {
                return new BadRequestObjectResult(new { errors = e.Message });
            }
        }
    }
}
