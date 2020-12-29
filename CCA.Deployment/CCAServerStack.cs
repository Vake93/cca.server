using Pulumi;
using Pulumi.Azure.AppService;
using Pulumi.Azure.AppService.Inputs;
using Pulumi.Azure.Core;
using Pulumi.Azure.Storage;
using System.Collections.Generic;

namespace CCA.Deployment
{
    public class CCAServerStack : Stack
    {
        public CCAServerStack()
        {
            var config = new Config();

            var resourceGroup = new ResourceGroup("cca-server", new ResourceGroupArgs { Location = "SouthIndia" });

            var storageAccount = new Account("ccastorage", new AccountArgs
            {
                ResourceGroupName = resourceGroup.Name,
                AccountReplicationType = "LRS",
                AccountTier = "Standard",
                AccountKind = "StorageV2"
            });

            var userService = CreateFunctionApp(
                "cca-user-service",
                config,
                resourceGroup,
                storageAccount,
                "../CCA.User.Service/bin/Release/netcoreapp3.1/publish",
                additionalSettings: new Dictionary<string, Output<string>>
                {
                    { "MicrosoftAuthClientId", Output.Create(config.Require("MICROSOFT_AUTH_CLIENTID"))},
                    { "MicrosoftAuthTenantId", Output.Create(config.Require("MICROSOFT_AUTH_TENANTID")) },
                    { "MicrosoftAuthClientSecret", config.RequireSecret("MICROSOFT_AUTH_CLIENT_SECRET") },
                    { "MicrosoftAuthUri", Output.Create(config.Require("MICROSOFT_AUTH_URI")) },

                    { "GoogleAuthClientId", Output.Create(config.Require("GOOGLE_AUTH_CLIENTID")) },
                    { "GoogleAuthClientSecret", config.RequireSecret("GOOGLE_AUTH_CLIENT_SECRET") },
                    { "GoogleAuthUri", Output.Create(config.Require("GOOGLE_AUTH_URI")) }
                });

            UserServiceEndpoint = Output.Format($"https://{userService.DefaultHostname}");

            var eventService = CreateFunctionApp(
                "cca-event-service",
                config,
                resourceGroup,
                storageAccount,
                "../CCA.Event.Service/bin/Release/netcoreapp3.1/publish");

            EventServiceEndpoint = Output.Format($"https://{eventService.DefaultHostname}");

            var meetingService = CreateFunctionApp(
                "cca-meeting-service",
                config,
                resourceGroup,
                storageAccount,
                "../CCA.Meeting.Service/bin/Release/netcoreapp3.1/publish");

            MeetingServiceEndpoint = Output.Format($"https://{meetingService.DefaultHostname}");

            var dialInService = CreateFunctionApp(
                "cca-dialin-service",
                config,
                resourceGroup,
                storageAccount,
                "../CCA.DialIn.Service/bin/Release/netcoreapp3.1/publish");

            DialInServiceEndpoint = Output.Format($"https://{dialInService.DefaultHostname}");

            var healthService = CreateFunctionApp(
                "cca-health-service",
                config,
                resourceGroup,
                storageAccount,
                "../CCA.Health.Service/bin/Release/netcoreapp3.1/publish",
                additionalSettings: new Dictionary<string, Output<string>>
                {
                    {"UserServiceUri"   , Output.Format($"https://{userService.DefaultHostname}/api/users/health-check") },
                    {"EventServiceUri"  , Output.Format($"https://{eventService.DefaultHostname}/api/events/health-check") },
                    {"MeetingServiceUri", Output.Format($"https://{meetingService.DefaultHostname}/api/meeting/health-check") },
                    {"DailInServiceUri" , Output.Format($"https://{dialInService.DefaultHostname}/api/dialin/health-check") }
                });

            HealthServiceEndpoint = Output.Format($"https://{healthService.DefaultHostname}/api/health-check");
        }

        [Output]
        public Output<string> UserServiceEndpoint { get; set; }

        [Output]
        public Output<string> EventServiceEndpoint { get; set; }

        [Output]
        public Output<string> MeetingServiceEndpoint { get; set; }

        [Output]
        public Output<string> DialInServiceEndpoint { get; set; }

        [Output]
        public Output<string> HealthServiceEndpoint { get; set; }

        private static FunctionApp CreateFunctionApp(
            string name,
            Config config,
            ResourceGroup resourceGroup,
            Account storageAccount,
            string codePath,
            Dictionary<string, Output<string>>? additionalSettings = null)
        {
            var userServicePlan = new Plan(name, new PlanArgs
            {
                ResourceGroupName = resourceGroup.Name,
                Kind = "FunctionApp",
                Sku = new PlanSkuArgs
                {
                    Tier = "Dynamic",
                    Size = "Y1"
                }
            });

            var container = new Container($"{name}-zips", new ContainerArgs
            {
                StorageAccountName = storageAccount.Name,
                ContainerAccessType = "private"
            });

            var blob = new Blob($"{name}-zip", new BlobArgs
            {
                StorageAccountName = storageAccount.Name,
                StorageContainerName = container.Name,
                Type = "Block",
                Source = new FileArchive(codePath)
            });

            var codeBlobUrl = SharedAccessSignature.SignedBlobReadUrl(blob, storageAccount);

            var appSettings = new InputMap<string>
            {
                { "FUNCTIONS_EXTENSION_VERSION", "~3" },
                { "FUNCTIONS_WORKER_RUNTIME", "dotnet" },
                { "WEBSITE_RUN_FROM_PACKAGE", codeBlobUrl},

                { "JwtSecretKey", config.RequireSecret("JWT_SECRET_KEY") },
                { "JwtIssuer", config.Require("JWT_ISSUER") },
                { "JwtAudience", config.Require("JWT_AUDIENCE")},
                { "JwtExpires", config.Require("JWT_EXPIRES") },

                { "TwilioAccountSid", config.Require("TWILIO_ACCOUNT_SID") },
                { "TwilioApiSid", config.Require("TWILIO_API_SID")},
                { "TwilioApiSecret", config.RequireSecret("TWILIO_API_SECRET") },
            };

            if (additionalSettings is { })
            {
                foreach (var setting in additionalSettings)
                {
                    appSettings.Add(setting.Key, setting.Value);
                }
            }

            return new FunctionApp(name, new FunctionAppArgs
            {
                ResourceGroupName = resourceGroup.Name,
                AppServicePlanId = userServicePlan.Id,
                StorageAccountName = storageAccount.Name,
                StorageAccountAccessKey = storageAccount.PrimaryAccessKey,
                Version = "~3",
                AppSettings = appSettings,
                ConnectionStrings = new InputList<FunctionAppConnectionStringArgs>
                {
                    new FunctionAppConnectionStringArgs
                    {
                        Name = "StorageConnectionString",
                        Type = "Custom",
                        Value = storageAccount.PrimaryConnectionString
                    }
                },
                SiteConfig = new FunctionAppSiteConfigArgs
                {
                    Cors = new FunctionAppSiteConfigCorsArgs
                    {
                        AllowedOrigins = new InputList<string>
                        {
                            "http://localhost:3000",
                            "https://cca.azureedge.net"
                        }
                    }
                }
            });
        }
    }
}
