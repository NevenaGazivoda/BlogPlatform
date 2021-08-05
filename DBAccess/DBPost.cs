using DBAccess.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess
{
    public class DBPost
    {
        string connectionString;
        SqlConnection db;

        public DBPost()
        {
            connectionString = DBConnection.conStr;
            db = new SqlConnection(connectionString);
        }
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

    }
}
