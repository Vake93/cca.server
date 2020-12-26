using Microsoft.AspNetCore.Mvc;
using Twilio.TwiML;

namespace CCA.Application.Extensions
{
    public class DialInVoiceResponse : VoiceResponse
    {
        public IActionResult ToWebResponse()
        {
            var result = new OkObjectResult(ToXml());
            result.ContentTypes.Clear();
            result.ContentTypes.Add("application/xml");

            return result;
        }
    }
}
