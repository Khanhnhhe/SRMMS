namespace SRMMS.DTOs
{
    public class PostDTO
    {
        public int PostId { get; set; }
        public string PostTitle { get; set; } = null!;


        public string EmpName { get; set; } = null!;
        public int PostToppicId { get; set; }
        public string PostImg { get; set; } = null!;

        public int EmpPostId { get; set; } 
        public string PostDetail { get; set; } = null!;
        public DateTime PostDate { get; set; }
        public string PostToppicName { get; set; } = null!;
    }
}
