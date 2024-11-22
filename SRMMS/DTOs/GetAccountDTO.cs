using System;
namespace SRMMS.DTOs
{
	public class GetAccountDTO
	{
        public int AccId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}

