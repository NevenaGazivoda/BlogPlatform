using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogPlatform.Models
{
    public class Welcome
    {
        public List<BlogPosts> blogPosts { get; set; }
        public Welcome()
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
