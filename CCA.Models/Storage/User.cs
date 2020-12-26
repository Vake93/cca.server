using Microsoft.Azure.Cosmos.Table;
using System;

namespace CCA.Models.Storage
{
    public class User : TableEntity
    {
        public User()
        {
            PartitionKey = "CCA";
            RowKey = Guid.NewGuid().ToString();
        }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string NormalizedEmail { get; set; } = string.Empty;

        public string PasswordSalt { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;
    }
}
