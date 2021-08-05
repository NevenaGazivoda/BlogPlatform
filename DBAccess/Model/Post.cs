using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Model
{
    public class blogPost
    {
        public string slug { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string body { get; set; }
        public List<string> tagList { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }

        public blogPost()
        {
            tagList = new List<string>();
        }
    }
    public class Root
    {
        public blogPost blogPost { get; set; }

        public Root()
        {
            blogPost = new blogPost();
        }
    }

}
