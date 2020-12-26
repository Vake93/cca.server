using CCA.Application.Configurations;
using CCA.Models.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using Twilio;
using Twilio.Http;
using Twilio.Jwt.AccessToken;
using Twilio.Rest.Video.V1;
using Twilio.TwiML.Voice;
using Task = System.Threading.Tasks.Task;

namespace CCA.Application.Services.Twilio
{
    public static class TwilioService
    {
        private const int _maxParticipants = 50;
        private const int _maxTokenDurationHours = 2;

        private static readonly TwilioConfiguration _twilioConfiguration = new TwilioConfiguration();

        static TwilioService()
        {
            TwilioClient.Init(
                _twilioConfiguration.ApiSid,
                _twilioConfiguration.ApiSecret,
                _twilioConfiguration.AccountSid);
        }

        public static Say.VoiceEnum Voice => Say.VoiceEnum.Woman;

        public static Say.LanguageEnum Language => Say.LanguageEnum.EnGb;

        public static List<Gather.InputEnum> PhoneInput => new List<Gather.InputEnum>
        {
            Gather.InputEnum.Dtmf,
        };

        public static async Task CreateMeetingRoomAsync(Event @event)
        {
            var uniqueName = GetRoomName(@event);

            var room = (await RoomResource.ReadAsync(
                uniqueName: uniqueName,
                limit: 1)).FirstOrDefault();

            if (room is null || room.UniqueName != uniqueName)
            {
                var callbackUrl = string.IsNullOrEmpty(_twilioConfiguration.RoomUpdateCallbackUrl) ?
                    null :
                    new Uri(_twilioConfiguration.RoomUpdateCallbackUrl);

                var callbackMethod = string.IsNullOrEmpty(_twilioConfiguration.RoomUpdateCallbackUrl) ?
                    null :
                    HttpMethod.Post;

                await RoomResource.CreateAsync(
                    uniqueName: uniqueName,
                    type: RoomResource.RoomTypeEnum.Group,
                    maxParticipants: _maxParticipants,
                    recordParticipantsOnConnect: false,
                    statusCallback: callbackUrl,
                    statusCallbackMethod: callbackMethod);
            }
        }

        public static string GetMeetingRoomJoinToken(
            Event @event,
            string userEmail)
        {
            if (@event is null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            var identity = !string.IsNullOrEmpty(userEmail) ?
                userEmail :
                throw new ArgumentNullException(nameof(userEmail));

            var grants = new HashSet<IGrant>
            {
                new VideoGrant
                {
                    Room = GetRoomName(@event),
                },
            };

            var duration = @event.EndTime - @event.StartTime;
            var expiration = duration <= TimeSpan.FromHours(_maxTokenDurationHours) ?
                DateTime.UtcNow.Add(duration) : DateTime.UtcNow.AddHours(_maxTokenDurationHours);

            var token = new Token(
                _twilioConfiguration.AccountSid,
                _twilioConfiguration.ApiSid,
                _twilioConfiguration.ApiSecret,
                identity,
                grants: grants,
                expiration: expiration);

            return token.ToJwt();
        }

        public static string GetRoomName(Event @event) =>
            $"ac-{@event?.RoomId ?? throw new ArgumentNullException(nameof(@event))}";

        public static bool ValidAccountSid(string accountSid) =>
            _twilioConfiguration.AccountSid == accountSid;
    }
}
