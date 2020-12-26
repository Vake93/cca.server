using CCA.Application.Extensions;
using CCA.Application.Services.Twilio;
using CCA.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using ConnectMeeting = Twilio.TwiML.Voice.Connect;
using EventDto = CCA.Models.Storage.Event;

namespace CCA.Application.Handlers.DialIn
{
    public class JoinMeeting
    {
        private readonly DialInJoinMeeting _joinMeetingRequest;
        private readonly CloudTable _eventsTable;
        private readonly ILogger _logger;

        public JoinMeeting(
            DialInJoinMeeting joinMeetingRequest,
            CloudTable eventsTable,
            ILogger logger)
        {
            _joinMeetingRequest = joinMeetingRequest;
            _eventsTable = eventsTable;
            _logger = logger;
        }

        public async Task<IActionResult> ProcessAsync()
        {
            var response = new DialInVoiceResponse();

            _logger.LogInformation(
                "Incoming call from {0} on account {1} for meeting {2}",
                _joinMeetingRequest.Caller,
                _joinMeetingRequest.AccountSid,
                _joinMeetingRequest.Digits);

            if (TwilioService.ValidAccountSid(_joinMeetingRequest.AccountSid))
            {
                var events = _eventsTable
                    .CreateQuery<EventDto>()
                    .AsQueryable()
                    .Where(e => e.RoomId == _joinMeetingRequest.Digits)
                    .ToArray();

                if (events.Length == 0)
                {
                    response
                        .Say(
                            message: "The code you entered is incorrect, " +
                            "please make sure to enter the 4 digit code " +
                            "provided in the event followed by " +
                            "hash key.",
                            language: TwilioService.Language,
                            voice: TwilioService.Voice,
                            loop: 1)
                        .Gather(
                            input: TwilioService.PhoneInput,
                            finishOnKey: "#",
                            timeout: 15,
                            actionOnEmptyResult: true,
                            action: new Uri("meeting", UriKind.Relative),
                            method: Twilio.Http.HttpMethod.Post);
                }
                else
                {
                    await TwilioService.CreateMeetingRoomAsync(events[0]);
                    var roomName = TwilioService.GetRoomName(events[0]);

                    response
                        .Say(
                            message: "Thank you, we are connecting you to the call.",
                            language: TwilioService.Language,
                            voice: TwilioService.Voice,
                            loop: 1)
                        .Append(new ConnectMeeting().Room(
                            roomName,
                            _joinMeetingRequest.Caller));
                }
            }
            else
            {
                response
                    .Say(
                        message: "Invalid account.",
                        language: TwilioService.Language,
                        voice: TwilioService.Voice,
                        loop: 1);
            }

            return response.ToWebResponse();
        }
    }
}
