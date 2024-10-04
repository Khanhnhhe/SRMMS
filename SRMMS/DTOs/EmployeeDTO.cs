namespace SRMMS.DTOs
{
    public class EmployeeDTO
    {
        public int EmpId { get; set; }
        public string? EmpFirstName { get; set; }
        public string? EmpLastName { get; set; }
        public string? EmpDob { get; set; }
        public string? EmpGender { get; set; }
        public string? EmpAdress { get; set; }
        public int? EmpPhoneNumber { get; set; }
        public string? EmpEmail { get; set; }
        public string? EmpStartDate { get; set; }
        public bool? EmpStatus { get; set; }
        public string? RoleName { get; set; }
    }
}
