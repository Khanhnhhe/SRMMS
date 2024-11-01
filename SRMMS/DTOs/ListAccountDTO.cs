namespace SRMMS.DTOs
{
    public class ListAccountDTO
    {

        public int AccountId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? RoleName { get; set; }
        public bool? Status { get; set; }
    }
}
