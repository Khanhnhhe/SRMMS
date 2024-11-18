namespace SRMMS.DTOs
{
    public class FeedbackOutputDto
    {
        public int FeedbackId { get; set; }
        public string Feedback1 { get; set; }
        public int RateStar { get; set; }
        public int AccId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public AccountDto Acc { get; set; }
    }
}
