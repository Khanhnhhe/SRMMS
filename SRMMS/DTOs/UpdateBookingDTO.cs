namespace SRMMS.DTOs
{
    public class UpdateBookingDTO
    {

        public int BookingId { get; set; }
        public DateTime? TimeBooking { get; set; }
        public int? NumberOfPeople { get; set; }
        public bool? Status { get; set; }
        public string? Shift { get; set; }
    }
}
