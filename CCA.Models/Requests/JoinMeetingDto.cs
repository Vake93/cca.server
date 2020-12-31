using Newtonsoft.Json;

namespace CCA.Models.Requests
{
    public class JoinMeetingDto
    {
        [JsonIgnore]
        public string EventId { get; set; } = string.Empty;
    }
}
