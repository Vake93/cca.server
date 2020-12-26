using CCA.Application.Services.Authentication.Google;
using CCA.Application.Services.Authentication.Microsoft;
using CCA.Models.Requests;
using CCA.Models.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CCA.Application.Handlers.Users
{
    public class OAuthUrl
    {
        private readonly OAuthUrlDto _oauthUrlRequest;
        private readonly ILogger _logger;

        public OAuthUrl(
            OAuthUrlDto oauthUrlRequest,
            ILogger logger)
        {
            _oauthUrlRequest = oauthUrlRequest;
            _logger = logger;
        }

        public Task<IActionResult> ProcessAsync()
        {
            try
            {
                Validator.ValidateRequest(_oauthUrlRequest);

                var url = _oauthUrlRequest.ProviderType switch
                {
                    AuthenticationProviderType.Google => GoogleAuthenticationService.Instance.GetLoginUrl(),
                    AuthenticationProviderType.Microsoft => MicrosoftAuthenticationService.Instance.GetLoginUrl(),
                    _ => string.Empty,
                };

                return Task.FromResult((IActionResult)new OkObjectResult(new { url }));
            }
            catch (ValidationException e)
            {
                _logger.LogWarning("Validation Error: {0}", e.Message);
                return Task.FromResult((IActionResult)new BadRequestObjectResult(new { errors = e.Message }));
            }
        }
    }
}
