using System;
using System.Collections.Generic;

namespace SRMMS.Models
{
    public partial class TopicOfPost
    {
        public TopicOfPost()
        {
            Posts = new HashSet<Post>();
        }

        public int TopicId { get; set; }
        public string TopicName { get; set; } = null!;

        public virtual ICollection<Post> Posts { get; set; }
    }
}
