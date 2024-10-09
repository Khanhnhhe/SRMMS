﻿using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class Post
    {
        public int PostId { get; set; }
        public string PostTitle { get; set; } = null!;
        public int EmpPostId { get; set; }
        public int PostToppicId { get; set; }
        public string PostImg { get; set; } = null!;
        public string PostDetail { get; set; } = null!;
        public DateTime PostDate { get; set; }

        public virtual Employee EmpPost { get; set; } = null!;
        public virtual TopicOfPost PostToppic { get; set; } = null!;
    }
}
