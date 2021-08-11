using Common.Post;
using Common.Tag;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BlogPlatform.Controllers
{
    [RoutePrefix("api/posts")]
    public class postsController : ApiController
    {
        string connectionString;
        SqlConnection db;

        public postsController()
        {
            connectionString = Connection.conStr;
            db = new SqlConnection(connectionString);
        }

        [Route("{slug}")]
        [HttpGet]
        public Root GetRoot(string slug)
        {
            DBPosts dbPost = new DBPosts();
            var rootObj = dbPost.GetBlogPost(slug);
            return rootObj;
        }

        [HttpGet]
        public BPosts GetPosts(string tag = null)
        {
            DBPosts dbPosts = new DBPosts();
            var bPosts = dbPosts.GetBlogPosts(tag);
            return bPosts;
        }
        
        [HttpPost]
        public void NewPost(Root post)
        {
            DBPosts bPosts = new DBPosts();
            bPosts.CreateNewPost(post);
        }

        [Route("{slug}")]
        [HttpDelete]
        public void DeletePost(string slug)
        {
            DBPosts dBPosts = new DBPosts();
            dBPosts.DeletePost(slug);
        }

        [Route("{slug}")]
        [HttpPut]
        public void UpdatePost (Root post, string slug)
        {
            DBPosts dBPosts = new DBPosts();
            dBPosts.UpdatePost(post, slug);
        }

        [Route("tags")]
        [HttpGet]
        public Tag GetTags()
        {
            DBTags dBTags = new DBTags();
            var list = dBTags.GetTags();
            return list;
        }
    }
}
