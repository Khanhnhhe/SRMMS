﻿namespace SRMMS.DTOs
{
    public class CreateEmployeeAccountDTO
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public int RoleId { get; set; }
    }
}
