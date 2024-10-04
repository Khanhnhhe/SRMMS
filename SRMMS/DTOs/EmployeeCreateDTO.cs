namespace SRMMS.DTOs
{
    public class EmployeeCreateDTO
    {
        public string EmpFirstName { get; set; } // Required
        public string EmpLastName { get; set; } // Required
        public DateTime? EmpDob { get; set; } // Optional
        public bool? EmpGender { get; set; } // Optional
        public int? EmpPhoneNumber { get; set; } // Optional
        public string? EmpEmail { get; set; } // Required (email is usually unique)
        public string? EmpPassword { get; set; } // Required
        public string? EmpAddress { get; set; } // Optional  
        public string? EmpWard { get; set; } // Optional
        public DateTime? EmpStartDate { get; set; } // Optional, default to DateTime.Now
        public bool? EmpStatus { get; set; } // Optional, default to true
        public string? RoleName { get; set; } // Optional, default to "Staff"
    }
}
