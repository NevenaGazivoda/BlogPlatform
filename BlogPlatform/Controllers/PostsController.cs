using BlogPlatform.Models;
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
        public Root GetBlogPost(string slug)
        {
            SqlCommand command = new SqlCommand("getPostBySlug", db)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@slug", SqlDbType.NVarChar).Value = slug;

            Root root = new Root();
            var n = 0;
            try
            {
                db.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    n = Convert.ToInt32(reader[0]);
                    root.blogPost.slug = Convert.ToString(reader[1]);
                    root.blogPost.title = Convert.ToString(reader[2]);
                    root.blogPost.description = Convert.ToString(reader[3]);
                    root.blogPost.body = Convert.ToString(reader[4]);
                    root.blogPost.createdAt = Convert.ToDateTime(reader[5]);
                    root.blogPost.updatedAt = Convert.ToDateTime(reader[6]);

                    //root.blogPost.tagList.Add(reader[10].ToString());

                }

                reader.Close();
                DBTags dbAcc = new DBTags();

                root.blogPost.tagList = dbAcc.CitanjeTagova(n);
                

                db.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return root;
        }


        [HttpGet]
        public Welcome GetBlogPosts(string tag = null)
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

                    DBTags dBAccess = new DBTags();
                    post.tagList = dBAccess.CitanjeTagova(post.postId);
                   
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

                blog.slug = post.slug;
                blog.title = post.title;
                blog.description = post.description;
                blog.body = post.body;
                blog.createdAt = post.createdAt;
                blog.updatedAt = post.updatedAt;
                blog.tagList = post.tagList;
                posts.blogPosts.Add(blog);
            }
            posts.postsCount = posts.blogPosts.Count;
            return posts;
        }

        [HttpPost]
        public void CreateNewPost(Root post)
        {

            post.blogPost.slug = post.blogPost.title.ToLower();
            post.blogPost.slug = post.blogPost.slug.Replace(" ", "-");

            SqlCommand command = new SqlCommand("createNewPost", db)
            {
                CommandType = CommandType.StoredProcedure
            };
            SqlParameter outputIdParam = new SqlParameter("@id", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add("@title", SqlDbType.VarChar).Value = post.blogPost.title;
            command.Parameters.Add("@description", SqlDbType.VarChar).Value = post.blogPost.description;
            command.Parameters.Add("@body", SqlDbType.VarChar).Value = post.blogPost.body;
            command.Parameters.Add("@slug", SqlDbType.NVarChar).Value = post.blogPost.slug;
            command.Parameters.Add(outputIdParam);
            var id = "";
            try
            {
                db.Open();
                command.ExecuteNonQuery();
                id = outputIdParam.Value.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            SqlCommand com = new SqlCommand("getTags", db)
            {
                CommandType = CommandType.StoredProcedure
            };

            List<tag> tags = new List<tag>();
            try
            {
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    tag t = new tag();
                    t.tagId = Convert.ToInt32(reader[0]);
                    t.name = Convert.ToString(reader[1]);
                    tags.Add(t);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            for (int i = 0; i < post.blogPost.tagList.Count; i++)
            {
                var idTag = "";
                //  tags.Where(tag => tag.name != post.blogPost.tagList[i]);
                if (!tags.Any(tag => tag.name == post.blogPost.tagList[i]))
                {
                    SqlCommand cmd = new SqlCommand("insertIntoTags", db)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    SqlParameter outputParam = new SqlParameter("@tagId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };

                    SqlParameter parameter = new SqlParameter();
                    parameter.ParameterName = "@name";
                    parameter.SqlDbType = SqlDbType.NVarChar;
                    parameter.Direction = ParameterDirection.Input;
                    parameter.Value = post.blogPost.tagList[i];


                    cmd.Parameters.Add(outputParam);
                    cmd.Parameters.Add(parameter);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        idTag = outputParam.Value.ToString();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else if (tags.Any(tag => tag.name == post.blogPost.tagList[i]))
                {
                    idTag = tags.Where(tag => tag.name == post.blogPost.tagList[i]).Select(tag => tag.tagId).Single().ToString();


                    ///////////

                }

                SqlCommand cm = new SqlCommand("insertIntoTagList", db)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cm.Parameters.Add("@idPost", SqlDbType.Int).Value = id;
                cm.Parameters.Add("@idTag", SqlDbType.Int).Value = idTag;

                try
                {
                    cm.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
            db.Close();
        }
               
        [Route("{slug}")]
        [HttpDelete]
        public void DeletePost(string slug)
        {
            SqlCommand command = new SqlCommand("deleteFromPosts", db)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@slug", SqlDbType.NVarChar).Value = slug;

            try
            {
                db.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            db.Close();
        }

        [Route("{slug}")]
        [HttpPut]
        public void UpdatePost (Root post, string slug)
        {

            SqlCommand command = new SqlCommand("getPostBySlug", db)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@slug", SqlDbType.NVarChar).Value = slug;

            Root blogP = new Root();
            var n = 0;
            try
            {
                db.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    n = Convert.ToInt32(reader[0]);
                    blogP.blogPost.slug = Convert.ToString(reader[1]);
                    blogP.blogPost.title = Convert.ToString(reader[2]);
                    blogP.blogPost.description = Convert.ToString(reader[3]);
                    blogP.blogPost.body = Convert.ToString(reader[4]);
                }

                reader.Close();
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            SqlCommand com = new SqlCommand("updateBlogPost", db)
                {
                    CommandType = CommandType.StoredProcedure
                };

            com.Parameters.Add("@id", SqlDbType.Int).Value = n;

            
            if (post.blogPost.title == null || post.blogPost.title == "")
            {
                com.Parameters.Add("@title", SqlDbType.NVarChar).Value = blogP.blogPost.title;
                com.Parameters.Add("@slug", SqlDbType.NVarChar).Value = blogP.blogPost.slug;
            }
            else
            {
                com.Parameters.Add("@title", SqlDbType.NVarChar).Value = post.blogPost.title;
                post.blogPost.slug = post.blogPost.title.ToLower();
                post.blogPost.slug = post.blogPost.slug.Replace(" ", "-");

                char[] arr = post.blogPost.slug.ToCharArray();

                arr = Array.FindAll<char>(arr, (c => (char.IsLetterOrDigit(c)
                                                  || char.IsWhiteSpace(c)
                                                  || c == '-')));
                post.blogPost.slug = new string(arr);

                com.Parameters.Add("@slug", SqlDbType.NVarChar).Value = post.blogPost.slug;
            }

            if (post.blogPost.description == null || post.blogPost.description == "")
            {
                com.Parameters.Add("@description", SqlDbType.NVarChar).Value = blogP.blogPost.description;
            }
            else
            {
                com.Parameters.Add("@description", SqlDbType.NVarChar).Value = post.blogPost.description;
            }
            if (post.blogPost.body == null || post.blogPost.body == "")
            {
                com.Parameters.Add("@body", SqlDbType.NVarChar).Value = blogP.blogPost.body;
            }
            else
            {
                com.Parameters.Add("@body", SqlDbType.NVarChar).Value = post.blogPost.body;
            }
            
                try
                {
                
                    com.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            db.Close();           
        }

        [Route("tags")]
        [HttpGet]
        public TagModel GetTags()
        {
            SqlCommand command = new SqlCommand("getTags", db)
            {
                CommandType = CommandType.StoredProcedure
            };
            TagModel tagsList = new TagModel();
            try
            {
                db.Open();
            SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    tagsList.Tags.Add(Convert.ToString(reader[1]));                
                }

                reader.Close();
                db.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return tagsList;
        }
    }
}
