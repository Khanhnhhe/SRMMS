namespace SRMMS.DTOs
{
    public class ChangeCustomerPasswordDTO
    {
        public int CusPhone { get; set; }
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
