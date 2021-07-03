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
            SqlCommand command = new SqlCommand("getPostBySlug", db)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@slug", SqlDbType.NVarChar).Value = slug;

            Root root = new Root();
           // List<string> sList = new List<string>();

            try
            {
                db.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    root.blogPost.slug = Convert.ToString(reader[1]);
                    root.blogPost.title = Convert.ToString(reader[2]);
                    root.blogPost.description = Convert.ToString(reader[3]);
                    root.blogPost.body = Convert.ToString(reader[4]);
                    root.blogPost.createdAt = Convert.ToDateTime(reader[5]);
                    root.blogPost.updatedAt = Convert.ToDateTime(reader[6]);

                    //sList.Add(reader[10].ToString());
                    root.blogPost.tagList.Add(reader[10].ToString());

                }

               // root.blogPost.tagList = sList;

                reader.Close();
                db.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return root;
        }
    }
}
