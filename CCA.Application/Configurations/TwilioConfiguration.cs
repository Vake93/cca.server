using System;

namespace CCA.Application.Configurations
{
    public class TwilioConfiguration
    {
        public string AccountSid => Environment.GetEnvironmentVariable("TwilioAccountSid");

        public string ApiSid => Environment.GetEnvironmentVariable("TwilioApiSid");

        public string ApiSecret => Environment.GetEnvironmentVariable("TwilioApiSecret");

        public string RoomUpdateCallbackUrl => Environment.GetEnvironmentVariable("TwilioRoomUpdateCallbackUrl");
    }
}
