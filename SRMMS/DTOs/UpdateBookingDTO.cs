namespace SRMMS.DTOs
{
    public class UpdateBookingDTO
    {

        public int BookingId { get; set; }

        public DateTime? DayBooking { get; set; }
        public string HourBooking { get; set; }
        public int? NumberOfPeople { get; set; }
        public int? StatusId { get; set; }
        public string? Shift { get; set; }
    }
}
