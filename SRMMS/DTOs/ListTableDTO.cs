namespace SRMMS.DTOs
{
    public class ListTableDTO
    {
        public int TableId { get; set; }
        public string? TableName { get; set; }

        public int? StatusId { get; set; }
        public string? StatusName { get; set; }
        public int? BookingId { get; set; }

        public string Shift { get; set; }
        public int? TableOfPeople { get; set; }


    }
}
