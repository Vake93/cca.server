using System;

namespace CCA.Models.Requests
{
    public class NewEventDto
    {
        public string Title { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string[] EventGuests { get; set; } = Array.Empty<string>();

        public string? Location { get; set; }

        public string? Notes { get; set; }
    }
}
