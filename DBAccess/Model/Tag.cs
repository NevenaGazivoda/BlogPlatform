using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Model
{
    public class Tag
    {
        public List<string> Tags { get; set; }

        public Tag()
        {
            Tags = new List<string>();
        }
    }

    public class TagModel
    {
        public int tagId { get; set; }
        public string name { get; set; }
        
    }
}
