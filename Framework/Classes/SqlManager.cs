using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Framework.Classes
{
    public static class SqlManager
    {
        public static SqlDataReader DoSqlCommandReader(string query, SqlConnection conn)
        {
            try
            {
                using (SqlCommand com = new SqlCommand(query, conn))
                {
                    return com.ExecuteReader();
                }
            }
            catch
            {
                return null;
            }
        }
        public static SqlDataReader DoSqlCommandReader(SqlCommand comm)
        {
            try
            {
                return comm.ExecuteReader();
            }
            catch
            {
                return null;
            }
        }

        public static bool DoSqlCommandNonQuery(string query, SqlConnection conn)
        {
            try
            {
                using (SqlCommand com = new SqlCommand(query, conn))
                {
                    com.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public static bool DoSqlCommandNonQuery(SqlCommand comm)
        {
            try
            {
                comm.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
