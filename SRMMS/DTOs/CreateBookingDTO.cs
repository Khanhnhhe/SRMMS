namespace SRMMS.DTOs
{
    public class CreateBookingDTO
    {
        public DateTime? DayBooking { get; set; }
        public string HourBooking { get; set; }
        public int? NumberOfPeople { get; set; }

        public string? NameBooking { get; set; }
        public string? PhoneBooking { get; set; }
    }
}
