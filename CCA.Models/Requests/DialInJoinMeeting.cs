namespace CCA.Models.Requests
{
    public class DialInJoinMeeting
    {
        public string AccountSid { get; set; } = string.Empty;

        public string Caller { get; set; } = string.Empty;

        public string Digits { get; set; } = string.Empty;
    }
}
