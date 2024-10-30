namespace SRMMS.DTOs
{
    public class ListTableDTO
    {
        public int Table_Id { get; set; }
        public string? Table_Name { get; set; } 
        public string? QR_Code { get; set; }
        public int? Acc_Id { get; set; }
        public DateTime? Time_Booking { get; set; }
        public bool? Status { get; set; }
    }
}
