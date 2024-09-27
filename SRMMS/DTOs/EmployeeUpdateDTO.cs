namespace SRMMS.DTOs;

public class EmployeeUpdateDTO
{
    public string? EmpFirstName { get; set; }
    public string? EmpLastName { get; set; }
    public DateTime? EmpDob { get; set; }
    public int? EmpPhoneNumber { get; set; }
    public string? EmpEmail { get; set; }
    public string? EmpPassword { get; set; }
    public DateTime? EmpStartDate { get; set; }
    public string? RoleName { get; set; } = "staff"; // Default role is "staff"
    public bool? EmpStatus { get; set; } 
}