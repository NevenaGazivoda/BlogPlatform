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

    }
}
