using System;

namespace CCA.Application.Configurations
{
    public class HealthCheckConfiguration
    {
        public string UserServiceUri => Environment.GetEnvironmentVariable("UserServiceUri");

        public string EventServiceUri => Environment.GetEnvironmentVariable("EventServiceUri");

        public string MeetingServiceUri => Environment.GetEnvironmentVariable("MeetingServiceUri");

        public string DialInServiceUri => Environment.GetEnvironmentVariable("DailInServiceUri");
    }
}
