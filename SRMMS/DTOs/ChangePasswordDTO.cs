namespace SRMMS.DTOs
{
    public class ChangePasswordDTO
    {
        public string? Phone { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

        public string ConfirmNewPassword { get; set; }
    }
}
