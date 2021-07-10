using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogPlatform.Models
{
    public class TagModel
    {
        public List<string> Tags { get; set; }

        public TagModel()
        {
            Tags = new List<string>();
        }
    }
}