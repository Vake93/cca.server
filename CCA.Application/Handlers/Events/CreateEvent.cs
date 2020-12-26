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
    public class CreateEvent
    {
        private readonly NewEventDto _newEventRequest;
        private readonly CloudTable _eventsTable;
        private readonly string _loginUserEmail;
        private readonly string _loginUserId;
        private readonly ILogger _logger;

        public CreateEvent(
            NewEventDto newEventRequest,
            CloudTable eventsTable,
            string loginUserEmail,
            string loginUserId,
            ILogger logger)
        {
            _newEventRequest = newEventRequest;
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
                Validator.ValidateRequest(_newEventRequest);

                _newEventRequest.EventGuests = _newEventRequest.EventGuests
                    .Append(_loginUserEmail)
                    .Select(eg => eg.ToUpperInvariant())
                    .Distinct()
                    .ToArray();

                var newEvent = new EventDto
                {
                    UserId = _loginUserId,
                    Title = _newEventRequest.Title,
                    StartTime = _newEventRequest.StartTime,
                    EndTime = _newEventRequest.EndTime,
                    EventGuests = _newEventRequest.EventGuests,
                    Notes = _newEventRequest.Notes,
                    Location = _newEventRequest.Location,
                    Timestamp = DateTime.UtcNow,
                };

                await _eventsTable.ExecuteAsync(TableOperation.Insert(newEvent));

                _logger.LogInformation("Created new event with id: {0}", newEvent.RowKey);

                return new CreatedResult(string.Empty, new { id = newEvent.RowKey });
            }
            catch (ValidationException e)
            {
                return new BadRequestObjectResult(new { errors = e.Message });
            }
        }
    }
}
