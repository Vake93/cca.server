using Newtonsoft.Json;
using System;

namespace CCA.Models.Requests
{
    public class UpdateEventDto
    {
        [JsonIgnore]
        public string Id { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string[] EventGuests { get; set; } = Array.Empty<string>();

        public string? Location { get; set; }

        public string? Notes { get; set; }
    }
}
