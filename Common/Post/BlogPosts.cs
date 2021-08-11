using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Post
{
    public class BPosts
    {
        public List<BlogPosts> blogPosts { get; set; }
        public BPosts()
        {
            blogPosts = new List<BlogPosts>();
        }
        public int postsCount { get; set; }

    }

    public class BlogPosts
    {
        public string slug { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string body { get; set; }
        public List<string> tagList { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }

        public BlogPosts()
        {
            tagList = new List<string>();
        }
    }
}
