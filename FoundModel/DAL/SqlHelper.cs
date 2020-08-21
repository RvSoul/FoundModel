using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoundModel.DAL
{
    public class SqlHelper
    {
        private readonly string SqlText;
        public SqlHelper(string sqltext)
        {
            SqlText = sqltext;
        }

        public T SqlTest<T>(string sql)
        {
            using (SqlConnection conn = new SqlConnection(SqlText))
            {
                conn.Open();

                SqlCommand comm = new SqlCommand(sql,conn);
                SqlDataReader dr = comm.ExecuteReader();
                 

                return JsonConvert.DeserializeObject<T>("");
            }
        }

        public T SqlTest<T>(string sql, string TableName)
        {
            using (SqlConnection conn = new SqlConnection(SqlText))
            {
                conn.Open();

                SqlDataAdapter sda = new SqlDataAdapter(sql, SqlText);
                DataSet ds = new DataSet();
                sda.Fill(ds, TableName);

                var a = ds.Tables[TableName];
                string data = JsonConvert.SerializeObject(ds.Tables[TableName]);

                return JsonConvert.DeserializeObject<T>(data);
            }
        }
    }
}
