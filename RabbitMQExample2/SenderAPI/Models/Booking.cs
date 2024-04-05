namespace SenderAPI.Models
{
    public class Booking
    {
        public string Id { get; set; }

        public string PassengerName { get; set; } = string.Empty;

        public string PassportNumber { get; set; } = string.Empty;

        public string SenderName { get; set; } = string.Empty;

        public string To { get; set; } = string.Empty;

        public int Status { get; set; }
    }
}
