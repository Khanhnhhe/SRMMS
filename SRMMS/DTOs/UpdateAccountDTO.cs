namespace SRMMS.DTOs
{
    public class UpdateAccountDTO
    {
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public int? RoleId { get; set; }

        public bool? Status { get; set; }

        //public DateTime? EndDate { get; set; }
    }
}
