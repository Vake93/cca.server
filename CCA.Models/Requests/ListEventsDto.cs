using System;

namespace CCA.Models.Requests
{
    public class ListEventsDto
    {
        public int Limit { get; set; } = 10;

        public int Skip { get; set; }

        public DateTime? Date { get; set; }
    }
}
