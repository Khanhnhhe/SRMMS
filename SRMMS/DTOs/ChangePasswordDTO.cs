namespace SRMMS.DTOs
{
    public class ChangePasswordDTO
    {
        public string EmpEmail { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
