namespace SRMMS.DTOs
{
    public class AccountPointDetailDTO
    {
        public int AccountId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public List<PointDetailDTO> Points { get; set; } = new List<PointDetailDTO>();
    }

    public class PointDetailDTO
    {
        public int PointId { get; set; } 
        public double? Points { get; set; }
    }
}
