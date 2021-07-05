using BlogPlatform.Models;
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
        public Root GetBlogPost(string slug)
        {
            SqlCommand command = new SqlCommand("gettPostBySlug", db)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@slug", SqlDbType.NVarChar).Value = slug;

            Root root = new Root();
            var n=0;            
            try
            {
                db.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    n= Convert.ToInt32(reader[0]);
                    root.blogPost.slug = Convert.ToString(reader[1]);
                    root.blogPost.title = Convert.ToString(reader[2]);
                    root.blogPost.description = Convert.ToString(reader[3]);
                    root.blogPost.body = Convert.ToString(reader[4]);
                    root.blogPost.createdAt = Convert.ToDateTime(reader[5]);
                    root.blogPost.updatedAt = Convert.ToDateTime(reader[6]);
                    
                    //root.blogPost.tagList.Add(reader[10].ToString());

                }

                reader.Close();

                SqlCommand com = new SqlCommand("getTagsforPost", db)
                {
                    CommandType = CommandType.StoredProcedure
                };

                com.Parameters.Add("@tagId", SqlDbType.Int).Value = n;
                try
                {
                    SqlDataReader r = com.ExecuteReader();
                    while (r.Read())
                    {
                        root.blogPost.tagList.Add(r[1].ToString());
                       
                    }
                    
                    r.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
              
                db.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return root;
        }
        

        [HttpGet]
        public Welcome GetBlogPosts(string tag="")
        {
            SqlCommand command = new SqlCommand("getPosts", db)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@tag", SqlDbType.NVarChar).Value = tag;

            List<PomocniBP> list = new List<PomocniBP>();

            try
            {
                db.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    PomocniBP post = new PomocniBP();

                    post.postId = Convert.ToInt32(reader[0]);
                    post.slug = Convert.ToString(reader[1]);
                    post.title = Convert.ToString(reader[2]);
                    post.description = Convert.ToString(reader[3]);
                    post.body = Convert.ToString(reader[4]);
                    post.createdAt = Convert.ToDateTime(reader[5]);
                    post.updatedAt = Convert.ToDateTime(reader[6]);

                    list.Add(post);
                }

                reader.Close();
                foreach (var post in list)
                {
                    SqlCommand com = new SqlCommand("getTagsforPost", db)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    com.Parameters.Add("@tagId", SqlDbType.Int).Value = post.postId;
                    try
                    {
                        SqlDataReader r = com.ExecuteReader();
                        while (r.Read())
                        {
                            post.tagList.Add(r[1].ToString());
                        }
                        r.Close();
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                db.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //List<Welcome> posts = new List<Welcome>();
            Welcome posts = new Welcome();

            foreach (var post in list)
            {
                BlogPosts blog = new BlogPosts();
                                         
                blog.slug= post.slug;
                blog.title = post.title;
                blog.description = post.description;
                blog.body = post.body;
                blog.createdAt = post.createdAt;
                blog.updatedAt = post.updatedAt;
                blog.tagList = post.tagList;
                posts.blogPosts.Add(blog);
                posts.postsCount = posts.blogPosts.Count;
            }
            return posts;
        }
    }
}
