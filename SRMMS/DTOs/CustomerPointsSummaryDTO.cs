public class CustomerPointsSummaryDTO
{
    public int AccountId { get; set; }
    public string FullName { get; set; }
    public int TotalPoints { get; set; }
    public List<PointDTO> Points { get; set; }
}
