using Newtonsoft.Json;

namespace CCA.Models.Requests
{
    public class DeleteEventDto
    {
        [JsonIgnore]
        public string Id { get; set; } = string.Empty;
    }
}
