using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class Post
    {
        public int PostId { get; set; }
        public string PostTitle { get; set; } = null!;
        public string EmpPostId { get; set; } = null!;
        public int PostToppicId { get; set; }
        public string PostImg { get; set; } = null!;
        public string PostDetail { get; set; } = null!;
        public DateTime PostDate { get; set; }

        public virtual Employee PostToppic { get; set; } = null!;
        public virtual TopicOfPost PostToppicNavigation { get; set; } = null!;
    }
}
