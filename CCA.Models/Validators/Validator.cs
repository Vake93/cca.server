using CCA.Models.Requests;
using System;
using System.Text;

namespace CCA.Models.Validators
{
    public static class Validator
    {
        public static bool ValidateRequest(LoginDto loginRequest)
        {
            var hasErrors = false;
            var errors = new StringBuilder();

            if (string.IsNullOrEmpty(loginRequest?.Email))
            {
                hasErrors = true;
                errors.AppendLine($"{nameof(loginRequest.Email)} is required.");
            }

            if (string.IsNullOrEmpty(loginRequest?.Password))
            {
                hasErrors = true;
                errors.AppendLine($"{nameof(loginRequest.Password)} is required.");
            }

            if (hasErrors)
            {
                throw new ValidationException(errors.ToString());
            }

            return true;
        }

        public static bool ValidateRequest(RegisterDto registerRequest)
        {
            var hasErrors = false;
            var errors = new StringBuilder();

            if (string.IsNullOrEmpty(registerRequest?.FirstName))
            {
                hasErrors = true;
                errors.AppendLine($"{nameof(registerRequest.FirstName)} is required.");
            }

            if (string.IsNullOrEmpty(registerRequest?.LastName))
            {
                hasErrors = true;
                errors.AppendLine($"{nameof(registerRequest.LastName)} is required.");
            }

            if (string.IsNullOrEmpty(registerRequest?.Email))
            {
                hasErrors = true;
                errors.AppendLine($"{nameof(registerRequest.Email)} is required.");
            }

            if (string.IsNullOrEmpty(registerRequest?.Password))
            {
                hasErrors = true;
                errors.AppendLine($"{nameof(registerRequest.Password)} is required.");
            }

            if (hasErrors)
            {
                throw new ValidationException(errors.ToString());
            }

            return true;
        }

        public static bool ValidateRequest(OAuthUrlDto oauthUrlRequest)
        {
            var hasErrors = false;
            var errors = new StringBuilder();

            if (string.IsNullOrEmpty(oauthUrlRequest?.Provider))
            {
                hasErrors = true;
                errors.AppendLine($"{nameof(oauthUrlRequest.Provider)} is required.");
            }
            else
            {
                if (Enum.TryParse(typeof(AuthenticationProviderType), oauthUrlRequest.Provider, out var providerType))
                {
                    oauthUrlRequest.ProviderType = (AuthenticationProviderType)providerType;
                }
                else
                {
                    hasErrors = true;
                    errors.AppendLine($"{nameof(oauthUrlRequest.Provider)} is invalid.");
                }
            }

            if (hasErrors)
            {
                throw new ValidationException(errors.ToString());
            }

            return true;
        }

        public static bool ValidateRequest(OAuthLoginDto loginRequest)
        {
            var hasErrors = false;
            var errors = new StringBuilder();

            if (string.IsNullOrEmpty(loginRequest?.Provider))
            {
                hasErrors = true;
                errors.AppendLine($"{nameof(loginRequest.Provider)} is required.");
            }
            else
            {
                if (Enum.TryParse(typeof(AuthenticationProviderType), loginRequest.Provider, out var providerType))
                {
                    loginRequest.ProviderType = (AuthenticationProviderType)providerType;
                }
                else
                {
                    hasErrors = true;
                    errors.AppendLine($"{nameof(loginRequest.Provider)} is invalid.");
                }
            }

            if (string.IsNullOrEmpty(loginRequest?.Token))
            {
                hasErrors = true;
                errors.AppendLine($"{nameof(loginRequest.Token)} is required.");
            }

            if (hasErrors)
            {
                throw new ValidationException(errors.ToString());
            }

            return true;
        }

        public static bool ValidateRequest(NewEventDto newEventRequest)
        {
            var hasErrors = false;
            var errors = new StringBuilder();

            if (string.IsNullOrEmpty(newEventRequest?.Title))
            {
                hasErrors = true;
                errors.AppendLine($"{nameof(newEventRequest.Title)} is required.");
            }

            if (newEventRequest?.StartTime >= newEventRequest?.EndTime)
            {
                hasErrors = true;
                errors.AppendLine("Invalid event start and end times.");
            }

            if (hasErrors)
            {
                throw new ValidationException(errors.ToString());
            }

            return true;
        }

        public static bool ValidateRequest(UpdateEventDto updateEventRequest)
        {
            var hasErrors = false;
            var errors = new StringBuilder();

            if (string.IsNullOrEmpty(updateEventRequest?.Id))
            {
                hasErrors = true;
                errors.AppendLine($"{nameof(updateEventRequest.Id)} is required.");
            }

            if (string.IsNullOrEmpty(updateEventRequest?.Title))
            {
                hasErrors = true;
                errors.AppendLine($"{nameof(updateEventRequest.Title)} is required.");
            }

            if (updateEventRequest?.StartTime >= updateEventRequest?.EndTime)
            {
                hasErrors = true;
                errors.AppendLine("Invalid event start and end times.");
            }

            if (hasErrors)
            {
                throw new ValidationException(errors.ToString());
            }

            return true;
        }

        public static bool ValidateRequest(DeleteEventDto deleteEventRequest)
        {
            var hasErrors = false;
            var errors = new StringBuilder();

            if (string.IsNullOrEmpty(deleteEventRequest?.Id))
            {
                hasErrors = true;
                errors.AppendLine($"{nameof(deleteEventRequest.Id)} is required.");
            }

            if (hasErrors)
            {
                throw new ValidationException(errors.ToString());
            }

            return true;
        }

        public static bool ValidateRequest(JoinMeetingDto joinMeetingRequest)
        {
            var hasErrors = false;
            var errors = new StringBuilder();

            if (string.IsNullOrEmpty(joinMeetingRequest?.RecaptchaToken))
            {
                hasErrors = true;
                errors.AppendLine($"{nameof(joinMeetingRequest.RecaptchaToken)} is required.");
            }

            if (hasErrors)
            {
                throw new ValidationException(errors.ToString());
            }

            return true;
        }


        public static bool ValidateRequest(TokenDto tokenRefreshRequest)
        {
            var hasErrors = false;
            var errors = new StringBuilder();

            if (string.IsNullOrEmpty(tokenRefreshRequest?.Token))
            {
                hasErrors = true;
                errors.AppendLine($"{nameof(tokenRefreshRequest.Token)} is required.");
            }

            if (string.IsNullOrEmpty(tokenRefreshRequest?.RefreshToken))
            {
                hasErrors = true;
                errors.AppendLine($"{nameof(tokenRefreshRequest.RefreshToken)} is required.");
            }

            if (hasErrors)
            {
                throw new ValidationException(errors.ToString());
            }

            return true;
        }
    }
}
