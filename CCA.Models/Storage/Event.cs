using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System;

namespace CCA.Models.Storage
{
    public class Event : TableEntity
    {
        private static readonly Random _random = new Random();

        public Event()
        {
            PartitionKey = "CCA";
            RowKey = Guid.NewGuid().ToString();
            RoomId = GetRandomRoomId();
        }

        public string UserId { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string[] EventGuests
        {
            get => JsonConvert.DeserializeObject<string[]>(EventGuestsData);
            set => EventGuestsData = JsonConvert.SerializeObject(value);
        }

        public string? Location { get; set; }

        public string? Notes { get; set; }

        public string RoomId { get; set; }

        [JsonIgnore]
        public string EventGuestsData { get; set; } = "[]";

        private static string GetRandomRoomId() => _random.Next(1000, 9999).ToString();
    }
}
