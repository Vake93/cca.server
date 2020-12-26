using CCA.Application.Services.Twilio;
using CCA.Models.Requests;
using CCA.Models.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using EventDto = CCA.Models.Storage.Event;

namespace CCA.Application.Handlers.Meetings
{
    public class JoinMeeting
    {
        private readonly JoinMeetingDto _joinMeetingRequest;
        private readonly CloudTable _eventsTable;
        private readonly string _loginUserEmail;
        private readonly ILogger _logger;

        public JoinMeeting(
            JoinMeetingDto joinMeetingRequest,
            CloudTable eventsTable,
            string loginUserEmail,
            ILogger logger)
        {
            _joinMeetingRequest = joinMeetingRequest;
            _loginUserEmail = loginUserEmail;
            _eventsTable = eventsTable;
            _logger = logger;
        }

        public async Task<IActionResult> ProcessAsync()
        {
            await _eventsTable.CreateIfNotExistsAsync();

            try
            {
                Validator.ValidateRequest(_joinMeetingRequest);

                var events = _eventsTable
                    .CreateQuery<EventDto>()
                    .AsQueryable()
                    .Where(e => e.RowKey == _joinMeetingRequest.EventId)
                    .ToArray();

                if (events.Length == 0 || !events[0].EventGuests.Contains(_loginUserEmail))
                {
                    return new NotFoundResult();
                }

                var @event = events[0];

                await TwilioService.CreateMeetingRoomAsync(@event);

                var meetingRoomToken = TwilioService.GetMeetingRoomJoinToken(@event, _loginUserEmail);

                _logger.LogInformation("User {0} joined meeting for event {1}", _loginUserEmail, @event.RowKey);

                return new OkObjectResult(new { @event, meetingRoomToken });
            }
            catch (ValidationException e)
            {
                return new BadRequestObjectResult(new { errors = e.Message });
            }
        }
    }
}
