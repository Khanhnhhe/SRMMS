namespace SRMMS.DTOs
{
    public class EmployeeDTO
    {
        public int EmpId { get; set; }
        public string EmpFirstName { get; set; }
        public string EmpLastName { get; set; }
        public DateTime? EmpDob { get; set; }
        public int? EmpPhoneNumber { get; set; }
        public string EmpEmail { get; set; }
        public DateTime? EmpStartDate { get; set; }
        public bool? EmpStatus { get; set; }
        public string? RoleName { get; set; }
    }
}
