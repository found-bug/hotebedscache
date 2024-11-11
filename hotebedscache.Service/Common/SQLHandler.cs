using Hotebedscache.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace Hotebedscache.Service.Common
{
    public class SQLHandler
    {
        internal string ConnectionString;

        public SQLHandler()
        {
            ConnectionString = new HotebedContext().Database.GetDbConnection().ConnectionString;
        }

        public object ExecuteNonQuery(SqlCommand cmd)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                cmd.Connection = conn;
                conn.Open();
                object value = cmd.ExecuteNonQuery();
                conn.Close();
                return value;
            }
        }

        public object ExecuteScalar(SqlCommand cmd)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                cmd.Connection = conn;
                conn.Open();
                object value = cmd.ExecuteScalar();
                conn.Close();
                return value;
            }
        }

        public DataSet GetDataSet(SqlCommand cmd)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                cmd.Connection = conn;
                using (SqlDataAdapter DA = new SqlDataAdapter(cmd))
                {
                    DataSet DS = new DataSet();
                    DA.Fill(DS, "list");
                    return DS;
                }
            }
        }

        public DataTable GetDataTable(SqlCommand cmd)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                cmd.Connection = conn;
                using (SqlDataAdapter DA = new SqlDataAdapter(cmd))
                {
                    DataSet DS = new DataSet();
                    DA.Fill(DS, "list");
                    return DS.Tables[0];
                }
            }
        }
    }
}
