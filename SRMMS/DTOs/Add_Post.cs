namespace SRMMS.DTOs
{
    public class AddPostDTO
    {
        public int PostId { get; set; }
        public string PostTitle { get; set; }
        public int EmpPostId { get; set; }
        public int PostToppicId { get; set; }
        public string PostImg { get; set; }
        public string PostDetail { get; set; }
        public DateTime PostDate { get; set; }
    }
}
