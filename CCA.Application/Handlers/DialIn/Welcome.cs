using CCA.Application.Extensions;
using CCA.Application.Services.Twilio;
using CCA.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace CCA.Application.Handlers.DialIn
{
    public class Welcome
    {
        private readonly DialInWelcome _joinMeetingRequest;
        private readonly ILogger _logger;

        public Welcome(
            DialInWelcome joinMeetingRequest,
            ILogger logger)
        {
            _joinMeetingRequest = joinMeetingRequest;
            _logger = logger;
        }

        public Task<IActionResult> ProcessAsync()
        {
            var response = new DialInVoiceResponse();

            _logger.LogInformation(
                "Incoming call from {0} on account {1}",
                _joinMeetingRequest.Caller,
                _joinMeetingRequest.AccountSid);

            if (TwilioService.ValidAccountSid(_joinMeetingRequest.AccountSid))
            {
                response
                    .Say(
                        message: "Welcome to the CCA Meetings. " +
                        "To join the call, please enter the 4 digit code " +
                        "provided in the event followed by hash key.",
                        language: TwilioService.Language,
                        voice: TwilioService.Voice,
                        loop: 1)
                    .Gather(
                        input: TwilioService.PhoneInput,
                        finishOnKey: "#",
                        timeout: 15,
                        actionOnEmptyResult: true,
                        action: new Uri("dialin/meeting", UriKind.Relative),
                        method: Twilio.Http.HttpMethod.Post);
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

            return Task.FromResult(response.ToWebResponse());
        }
    }
}
