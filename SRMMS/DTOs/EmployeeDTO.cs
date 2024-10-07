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
        public int EmpRoleId { get; set; }
        public bool? EmpStatus { get; set; }
        public RoleDTO EmpRole { get; set; }

    }
    public class RoleDTO
    {
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? Description { get; set; }
    }
}
