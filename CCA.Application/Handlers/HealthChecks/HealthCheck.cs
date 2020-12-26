using CCA.Application.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CCA.Application.Handlers.HealthChecks
{
    public class HealthCheck
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private readonly ILogger _logger;
        private readonly string _remoteAddress;
        private readonly HealthCheckConfiguration _healthCheckConfiguration;

        public HealthCheck(
            string remoteAddress,
            ILogger logger)
        {
            _healthCheckConfiguration = new HealthCheckConfiguration();
            _remoteAddress = remoteAddress;
            _logger = logger;
        }

        public async Task<IActionResult> ProcessAsync()
        {
            try
            {
                var userServiceHealth = _httpClient.GetStringAsync(_healthCheckConfiguration.UserServiceUri);
                var eventServiceHealth = _httpClient.GetStringAsync(_healthCheckConfiguration.EventServiceUri);
                var meetingServiceHealth = _httpClient.GetStringAsync(_healthCheckConfiguration.MeetingServiceUri);
                var dailInServiceHealth = _httpClient.GetStringAsync(_healthCheckConfiguration.DialInServiceUri);

                await Task.WhenAll(userServiceHealth, eventServiceHealth, meetingServiceHealth, dailInServiceHealth);

                _logger.LogInformation($"Running health-check API for {_remoteAddress}");

                return new OkObjectResult(new { status = "Healthy" });
            }
            catch (Exception)
            {
                return new ObjectResult(new { status = "Unhealthy" })
                {
                    StatusCode = 500
                };
            }
        }
    }
}
