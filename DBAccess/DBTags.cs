using Common.Tag;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess
{
    public class DBTags
    {

        string connectionString;
        SqlConnection db;

        public DBTags()
        {
            connectionString = DBConnection.conStr;
            db = new SqlConnection(connectionString);
        }

        public List<string> CitanjeTagova(int tagID)
        {
            List<string> listaTagova = new List<string>();

            SqlCommand com = new SqlCommand("getTagsforPost", db)
            {
                CommandType = CommandType.StoredProcedure
            };

            com.Parameters.Add("@tagId", SqlDbType.Int).Value = tagID;
            try
            {
                db.Open();
                SqlDataReader r = com.ExecuteReader();
                while (r.Read())
                {
                    listaTagova.Add(r[1].ToString());

                }

                r.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            db.Close();

            return listaTagova;
        }

        public Tag GetTags()
        {
            SqlCommand command = new SqlCommand("getTags", db)
            {
                CommandType = CommandType.StoredProcedure
            };
            Tag tagsList = new Tag();
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

        public List<TagModel> GetAllTags()
        {

            SqlCommand com = new SqlCommand("getTags", db)
            {
                CommandType = CommandType.StoredProcedure
            };

            List<TagModel> tags = new List<TagModel>();
            try
            {
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    TagModel t = new TagModel();
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
            return tags;
        }

        public string InsertTag (string name)
        {
            SqlCommand cmd = new SqlCommand("insertIntoTags", db)
            {
                CommandType = CommandType.StoredProcedure
            };
            SqlParameter outputParam = new SqlParameter("@tagId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            var idTag = "";

            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@name";
            parameter.SqlDbType = SqlDbType.NVarChar;
            parameter.Direction = ParameterDirection.Input;
            parameter.Value = name;


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
            return idTag;
        }
    }
}
